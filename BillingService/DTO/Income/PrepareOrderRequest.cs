namespace BillingService.DTO.Income;

public record PrepareOrderRequest(Guid UserId, Guid OrderId, int Quantity, decimal FullCost);
