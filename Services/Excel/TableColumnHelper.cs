using ClosedXML.Excel;

namespace ExcelOrderSync.Function.Services.Excel;

public static class TableColumnHelper
{
    public static int GetColumn(IXLTable table, string columnName)
    {
        var field = table.Fields
            .First(f => f.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

        return field.Index + 1;
    }
}
