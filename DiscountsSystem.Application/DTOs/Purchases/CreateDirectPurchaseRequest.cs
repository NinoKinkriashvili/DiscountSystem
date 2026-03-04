namespace DiscountsSystem.Application.DTOs.Purchases;

public sealed record CreateDirectPurchaseRequest(
    int OfferId,
    int Quantity
);