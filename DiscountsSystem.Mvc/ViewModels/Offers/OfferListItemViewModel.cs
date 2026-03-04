namespace DiscountsSystem.Mvc.ViewModels.Offers;

public sealed class OfferListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal OriginalPrice { get; set; }

    public decimal DiscountPrice { get; set; }

    public DateTime EndDateUtc { get; set; }
}
