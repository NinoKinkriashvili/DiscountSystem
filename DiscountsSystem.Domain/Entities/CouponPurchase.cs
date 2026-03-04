using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Domain.Entities;

public sealed class CouponPurchase
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = default!;

    public int OfferId { get; set; }
    public Offer? Offer { get; set; }
    public int Quantity { get; set; }
    public int? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
    public PurchaseSourceType SourceType { get; set; } = PurchaseSourceType.Direct;
    public CouponPurchaseStatus Status { get; set; } = CouponPurchaseStatus.Active;

    public string CouponCode { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
