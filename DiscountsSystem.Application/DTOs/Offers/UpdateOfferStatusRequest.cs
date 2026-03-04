using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Offers;

public record UpdateOfferStatusRequest(
    string? Status,
    string? RejectReason
);