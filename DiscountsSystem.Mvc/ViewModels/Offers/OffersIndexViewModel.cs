namespace DiscountsSystem.Mvc.ViewModels.Offers;

public sealed class OffersIndexViewModel
{
    public List<OfferListItemViewModel> Items { get; set; } = [];

    public string? ErrorMessage { get; set; }
}
