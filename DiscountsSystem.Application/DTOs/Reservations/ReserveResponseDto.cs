using DiscountsSystem.Application.DTOs.Reservations.Results;

namespace DiscountsSystem.Application.DTOs.Reservations;

public sealed record ReserveResponseDto(
    ReserveResult Result,
    int? ReservationId
);