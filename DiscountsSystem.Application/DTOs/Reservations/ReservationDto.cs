using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Reservations;

public record ReservationDto(
    int Id,
    int OfferId,
    int Quantity,
    ReservationStatus Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime ExpiresAtUtc
);