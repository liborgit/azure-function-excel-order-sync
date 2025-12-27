using ExcelOrderSync.Function.Models;

public class OrderRowOperation
{
    public OrderRowAction Action { get; set; } = OrderRowAction.Unknown;
    public string? OrderLineId { get; set; }

    public string? OrderId { get; set; }
    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public string? Status { get; set; }
}
