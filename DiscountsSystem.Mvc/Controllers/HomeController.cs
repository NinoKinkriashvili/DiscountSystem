using DiscountsSystem.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Mvc.Controllers;

public sealed class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IHttpClientFactory httpClientFactory,
        ILogger<HomeController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new HomeIndexViewModel
        {
            Message = "MVC is running."
        };

        try
        {
            var client = _httpClientFactory.CreateClient("DiscountsApi");

            var response = await client.GetAsync("/health", ct);

            vm.ApiReachable = response.IsSuccessStatusCode;
            vm.Message = vm.ApiReachable
                ? "MVC -> API connection works."
                : $"API responded with status: {(int)response.StatusCode}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call API from MVC.");
            vm.ApiReachable = false;
            vm.Message = "Failed to reach API. Check if API is running and BaseUrl is correct.";
        }

        return View(vm);
    }
}
