using ClosedXML.Excel;

namespace ExcelOrderSync.Function.Services.Excel;

public static class TableLookupHelper
{
    public static IXLRangeRow? FindRow(IXLTable table, string orderLineId)
    {
        var idCol = TableColumnHelper.GetColumn(table, "OrderLineId");

        return table.DataRange.Rows()
            .FirstOrDefault(r =>
                r.Cell(idCol).GetString()
                .Equals(orderLineId, StringComparison.OrdinalIgnoreCase));
    }
}
