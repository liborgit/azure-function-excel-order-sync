using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using ExcelOrderSync.Function.Models;
using ExcelOrderSync.Function.Services;

namespace ExcelOrderSync.Function.Functions;

public class OrderBatchUpdateFunction
{
    private readonly BlobStorageService _blob;
    private readonly ExcelOrderLineProcessor _processor;
    private readonly OrderInputValidator _validator;
    private readonly ILogger _logger;
    private readonly string _sheetName;
    private readonly string _tableName;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public OrderBatchUpdateFunction(
        BlobStorageService blob,
        ExcelOrderLineProcessor processor,
        OrderInputValidator validator,
        IConfiguration config,
        ILoggerFactory loggerFactory)
    {
        _blob = blob;
        _processor = processor;
        _validator = validator;
        _logger = loggerFactory.CreateLogger<OrderBatchUpdateFunction>();

        _sheetName = config["ExcelSheetName"]
                     ?? throw new InvalidOperationException("Missing ExcelSheetName");

        _tableName = config["ExcelTableName"]
                     ?? throw new InvalidOperationException("Missing ExcelTableName");
    }

    [Function("OrderBatchUpdate")]
    public async Task Run(
        [ServiceBusTrigger("%ServiceBusInputQueue%", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage sbMessage,
        ServiceBusMessageActions actions)
    {
        _logger.LogInformation("Processing message {MessageId}", sbMessage.MessageId);

        var body = sbMessage.Body.ToString();

        try
        {
            var payload = JsonSerializer.Deserialize<OrderBatchUpdateMessage>(body, JsonOptions)
                         ?? throw new InvalidOperationException("Input payload is empty or invalid.");

            _validator.Validate(payload);

            await using var stream = await _blob.DownloadAsync();
            using var workbook = new XLWorkbook(stream);

            var sheet = workbook.Worksheet(_sheetName)
                       ?? throw new InvalidOperationException($"Worksheet '{_sheetName}' not found.");

            var table = sheet.Table(_tableName)
                       ?? throw new InvalidOperationException($"Excel table '{_tableName}' not found.");

            _processor.Process(table, payload.OrderUpdates);

            using var output = new MemoryStream();
            workbook.SaveAs(output);
            output.Position = 0;

            await _blob.UploadAsync(null, output);

            _logger.LogInformation("Excel updated successfully.");
        }
        
        catch (Exception ex)
        {
            var description = ex.ToString();
            const int maxLen = 3500; // safe size
            if (description.Length > maxLen)
                description = description.Substring(0, maxLen);

            await actions.DeadLetterMessageAsync(
                sbMessage,
                deadLetterReason: ex.GetType().Name,
                deadLetterErrorDescription: description);

            _logger.LogError(ex, "Message moved to DLQ.");
        }
    }
}
