namespace DiscountsSystem.Application.DTOs.Reservations;

public record CreateReservationRequest(
    int OfferId,
    int Quantity
);