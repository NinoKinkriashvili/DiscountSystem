namespace DiscountsSystem.Application.DTOs.Purchases.Results;

public enum PurchaseResult
{
    Success = 0,

    // Common
    OfferNotFound = 1,
    OfferNotPurchasable = 2,
    NotEnoughCoupons = 3,
    ConcurrencyConflict = 4,

    // From reservation flow
    ReservationNotFound = 5,
    ReservationNotOwnedByCustomer = 6,
    ReservationNotActive = 7,
    ReservationExpired = 8,
    ReservationOfferMismatch = 9,

    // Generic fallback
    UnexpectedError = 10,

    // Specific business case
    ReservationAlreadyPurchased = 11
}