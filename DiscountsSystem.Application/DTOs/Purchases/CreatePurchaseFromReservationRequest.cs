namespace DiscountsSystem.Application.DTOs.Purchases;

public sealed record CreatePurchaseFromReservationRequest(
    int ReservationId
);