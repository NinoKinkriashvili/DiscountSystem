using DiscountsSystem.Application.DTOs.Purchases.Results;

namespace DiscountsSystem.Application.DTOs.Purchases;

public sealed record PurchaseResponseDto(
    PurchaseResult Result,
    int? PurchaseId
);