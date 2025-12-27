using ExcelOrderSync.Function.Services;
using ExcelOrderSync.Function.Services.Operations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Blob service
builder.Services.AddSingleton<BlobStorageService>();

// Validator
builder.Services.AddSingleton<OrderInputValidator>();

// Orchestrator for Excel operations
builder.Services.AddSingleton<ExcelOrderLineProcessor>();

// Excel operations
builder.Services.AddSingleton<AddOrderLineService>();
builder.Services.AddSingleton<UpdateOrderLineService>();
builder.Services.AddSingleton<DeleteOrderLineService>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
