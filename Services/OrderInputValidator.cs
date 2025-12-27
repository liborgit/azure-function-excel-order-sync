using ExcelOrderSync.Function.Models;

namespace ExcelOrderSync.Function.Services;

public class OrderInputValidator
{
    public void Validate(OrderBatchUpdateMessage payload)
    {
        // Payload must contain at least one update
        if (payload.OrderUpdates == null || payload.OrderUpdates.Count == 0)
            throw new InvalidOperationException("No order updates found in message.");

        for (int i = 0; i < payload.OrderUpdates.Count; i++)
        {
            var op = payload.OrderUpdates[i];

            // Action must be provided and valid
            if (op.Action == OrderRowAction.Unknown)
                throw new InvalidOperationException(
                    $"OrderUpdates[{i}]: Action is missing or invalid.");

            // OrderLineId is required for all operations
            if (string.IsNullOrWhiteSpace(op.OrderLineId))
                throw new InvalidOperationException(
                    $"OrderUpdates[{i}]: OrderLineId is missing.");

            // Add operation requires all mandatory fields
            if (op.Action == OrderRowAction.Add)
            {
                if (string.IsNullOrWhiteSpace(op.OrderId))
                    throw new InvalidOperationException(
                        $"OrderUpdates[{i}] (Add): OrderId is required.");

                if (string.IsNullOrWhiteSpace(op.ProductName))
                    throw new InvalidOperationException(
                        $"OrderUpdates[{i}] (Add): ProductName is required.");

                if (op.Quantity is null)
                    throw new InvalidOperationException(
                        $"OrderUpdates[{i}] (Add): Quantity is required.");

                if (op.Price is null)
                    throw new InvalidOperationException(
                        $"OrderUpdates[{i}] (Add): Price is required.");
            }

            // Update operation must change at least one field
            if (op.Action == OrderRowAction.Update)
            {
                if (op.OrderId is null &&
                    op.ProductName is null &&
                    op.Quantity is null &&
                    op.Price is null &&
                    op.Status is null)
                {
                    throw new InvalidOperationException(
                        $"OrderUpdates[{i}] (Update): No fields to update.");
                }
            }
        }
    }
}
