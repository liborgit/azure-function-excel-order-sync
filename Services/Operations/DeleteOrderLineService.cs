using ClosedXML.Excel;
using ExcelOrderSync.Function.Services.Excel;

namespace ExcelOrderSync.Function.Services.Operations;

public class DeleteOrderLineService
{
    public void Execute(IXLTable table, OrderLineColumns cols, OrderRowOperation u)
    {
        if (string.IsNullOrWhiteSpace(u.OrderLineId))
            throw new InvalidOperationException("OrderLineId is missing.");

        var row = TableLookupHelper.FindRow(table, u.OrderLineId)
                 ?? throw new InvalidOperationException(
                     $"Row with ID {u.OrderLineId} does not exist.");

        row.Delete();
    }
}
