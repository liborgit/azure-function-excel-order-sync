using ClosedXML.Excel;
using ExcelOrderSync.Function.Services.Excel;

namespace ExcelOrderSync.Function.Services.Operations;

public class UpdateOrderLineService
{
    public void Execute(IXLTable table, OrderLineColumns cols, OrderRowOperation u)
    {
        var row = TableLookupHelper.FindRow(table, u.OrderLineId)
                 ?? throw new InvalidOperationException($"Row with ID {u.OrderLineId} does not exist.");

        if (u.OrderId != null) row.Cell(cols.OrderId).Value = u.OrderId;
        if (u.ProductName != null) row.Cell(cols.ProductName).Value = u.ProductName;
        if (u.Quantity != null) row.Cell(cols.Quantity).Value = u.Quantity;
        if (u.Price != null) row.Cell(cols.Price).Value = u.Price;
        if (u.Status != null) row.Cell(cols.Status).Value = u.Status;

        try
        {
            int qty = row.Cell(cols.Quantity).GetValue<int>();
            decimal price = row.Cell(cols.Price).GetValue<decimal>();
            row.Cell(cols.Total).Value = qty * price;
        }
        
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Invalid data for OrderLineId {u.OrderLineId}", ex);
        }

        row.Cell(cols.UpdatedAt).Value = DateTime.UtcNow;
    }
}
