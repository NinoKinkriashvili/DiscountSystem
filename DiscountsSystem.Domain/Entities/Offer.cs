using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Domain.Entities;

public sealed class Offer
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal OriginalPrice { get; set; }
    public decimal DiscountPrice { get; set; }

    public int CouponQuantityTotal { get; set; }
    public int CouponQuantityAvailable { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public string? RejectReason { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; } = null!;

    public string MerchantId { get; set; } = string.Empty;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public DateTime? ApprovedAtUtc { get; set; }
}
