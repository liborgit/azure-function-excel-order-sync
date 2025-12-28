# Technical notes

### Configuration for local testing

Create `local.settings.json` and set the values according to your environment.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "ServiceBusConnection": "<SERVICE_BUS_CONNECTION_STRING>",
    "ServiceBusInputQueue": "<SERVICE_BUS_QUEUE_NAME>",

    "BlobConnectionString": "<BLOB_CONNECTION_STRING>",
    "BlobContainerName": "<BLOB_CONTAINER_NAME>",
    "ExcelFilePath": "<EXCEL_FILE_PATH>",

    "ExcelSheetName": "<EXCEL_SHEET_NAME>",
    "ExcelTableName": "<EXCEL_TABLE_NAME>"
  }
}
```

---

### Configuration keys for cloud

| Key | Purpose |
|---|---|
| `AzureWebJobsStorage` | Storage account used by Azure Functions runtime |
| `FUNCTIONS_WORKER_RUNTIME` | Specifies the Functions runtime (`dotnet-isolated`) |
| `ServiceBusConnection` | Azure Service Bus connection string |
| `ServiceBusInputQueue` | Name of the input queue |
| `BlobConnectionString` | Azure Blob Storage connection string |
| `BlobContainerName` | Blob container name |
| `ExcelFilePath` | Excel file path inside the container |
| `ExcelSheetName` | Worksheet containing the Excel table |
| `ExcelTableName` | Excel table name |
