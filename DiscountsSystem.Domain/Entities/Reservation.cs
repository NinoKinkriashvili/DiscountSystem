using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Domain.Entities;

public sealed class Reservation
{
    public int Id { get; set; }

    public int OfferId { get; set; }
    public Offer? Offer { get; set; }

    public string CustomerId { get; set; } = default!;

    public int Quantity { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    
}