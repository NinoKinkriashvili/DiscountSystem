using System.Text.Json;
using DiscountsSystem.Mvc.ViewModels.Offers;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Mvc.Controllers;

public sealed class OffersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OffersController> _logger;

    public OffersController(
        IHttpClientFactory httpClientFactory,
        ILogger<OffersController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new OffersIndexViewModel();

        try
        {
            var client = _httpClientFactory.CreateClient("DiscountsApi");

            var response = await client.GetAsync("/api/v1/public/offers", ct);

            if (!response.IsSuccessStatusCode)
            {
                vm.ErrorMessage = $"API returned status: {(int)response.StatusCode}";
                return View(vm);
            }

            var json = await response.Content.ReadAsStringAsync(ct);

            var apiItems = JsonSerializer.Deserialize<List<OfferListItemApiDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (apiItems is null)
            {
                vm.ErrorMessage = "API returned empty or invalid data.";
                return View(vm);
            }

            vm.Items = apiItems.Select(x => new OfferListItemViewModel
            {
                Id = x.Id,
                Title = x.Title ?? string.Empty,
                OriginalPrice = x.OriginalPrice,
                DiscountPrice = x.DiscountPrice,
                EndDateUtc = x.EndDateUtc
            }).ToList();
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            _logger.LogWarning("Offers/Index request was canceled.");
            vm.ErrorMessage = "Request was canceled.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load offers from API.");
            vm.ErrorMessage = "Failed to load offers from API. Check API route and response format.";
        }

        return View(vm);
    }

    private sealed class OfferListItemApiDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime EndDateUtc { get; set; }
    }
}
