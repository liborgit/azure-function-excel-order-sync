namespace ExcelOrderSync.Function.Models;

public class OrderBatchUpdateMessage
{
    public List<OrderRowOperation> OrderUpdates { get; set; } = new();
}
