using ClosedXML.Excel;
using ExcelOrderSync.Function.Services.Excel;

namespace ExcelOrderSync.Function.Services.Operations;

public class AddOrderLineService
{
    public void Execute(IXLTable table, OrderLineColumns cols, OrderRowOperation u)
    {
        var existing = TableLookupHelper.FindRow(table, u.OrderLineId);
        if (existing != null)
            throw new InvalidOperationException($"Row with ID {u.OrderLineId} already exists.");

        var newRow = table.DataRange.InsertRowsBelow(1).First();

        newRow.Cell(cols.OrderLineId).Value = u.OrderLineId;
        newRow.Cell(cols.OrderId).Value = u.OrderId;
        newRow.Cell(cols.ProductName).Value = u.ProductName;

        var qty = u.Quantity.GetValueOrDefault();
        var price = u.Price.GetValueOrDefault();

        newRow.Cell(cols.Quantity).Value = qty;
        newRow.Cell(cols.Price).Value = price;
        newRow.Cell(cols.Total).Value = qty * price;

        newRow.Cell(cols.Status).Value = u.Status;
        newRow.Cell(cols.UpdatedAt).Value = DateTime.UtcNow;
    }
}
