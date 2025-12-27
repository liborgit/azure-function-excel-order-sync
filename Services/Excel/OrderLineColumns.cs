namespace ExcelOrderSync.Function.Services.Excel;

public sealed record OrderLineColumns(
    int OrderLineId,
    int OrderId,
    int ProductName,
    int Quantity,
    int Price,
    int Total,
    int Status,
    int UpdatedAt);
