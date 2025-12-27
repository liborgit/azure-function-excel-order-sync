using ClosedXML.Excel;
using ExcelOrderSync.Function.Models;
using ExcelOrderSync.Function.Services.Excel;
using ExcelOrderSync.Function.Services.Operations;

namespace ExcelOrderSync.Function.Services;

public class ExcelOrderLineProcessor
{
    private readonly AddOrderLineService _add;
    private readonly UpdateOrderLineService _update;
    private readonly DeleteOrderLineService _delete;

    public ExcelOrderLineProcessor(
        AddOrderLineService add,
        UpdateOrderLineService update,
        DeleteOrderLineService delete)
    {
        _add = add;
        _update = update;
        _delete = delete;
    }

    public void Process(IXLTable table, IEnumerable<OrderRowOperation> updates)
    {
        var cols = new OrderLineColumns(
            OrderLineId: TableColumnHelper.GetColumn(table, "OrderLineId"),
            OrderId: TableColumnHelper.GetColumn(table, "OrderId"),
            ProductName: TableColumnHelper.GetColumn(table, "ProductName"),
            Quantity: TableColumnHelper.GetColumn(table, "Quantity"),
            Price: TableColumnHelper.GetColumn(table, "Price"),
            Total: TableColumnHelper.GetColumn(table, "Total"),
            Status: TableColumnHelper.GetColumn(table, "Status"),
            UpdatedAt: TableColumnHelper.GetColumn(table, "UpdatedAt")
        );

        foreach (var u in updates)
        {
            switch (u.Action)
            {
                case OrderRowAction.Add:
                    _add.Execute(table, cols, u);
                    break;

                case OrderRowAction.Update:
                    _update.Execute(table, cols, u);
                    break;

                case OrderRowAction.Delete:
                    _delete.Execute(table, cols, u);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown action: {u.Action}");
            }
        }
    }
}
