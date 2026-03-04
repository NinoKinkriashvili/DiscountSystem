namespace DiscountsSystem.Application.DTOs.Reservations.Results;

public enum ReserveResult
{
    Success = 0,

    OfferNotFound = 1,
    OfferNotReservable = 2,

    NotEnoughCoupons = 3,
    AlreadyHasActiveReservation = 4,

    ConcurrencyConflict = 5,
    UnexpectedError = 6
}
