using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.MerchantDashboard;

public sealed record MerchantOfferStatusCountDto(
    OfferStatus Status,
    int Count
);