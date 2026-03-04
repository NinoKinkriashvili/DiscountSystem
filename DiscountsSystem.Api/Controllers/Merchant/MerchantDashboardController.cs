using Asp.Versioning;
using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Merchant;

[ApiController]
[Authorize(Roles = "Merchant")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/merchant/dashboard")]
public sealed class MerchantDashboardController : ControllerBase
{
    private readonly IMerchantDashboardService _merchantDashboardService;

    public MerchantDashboardController(IMerchantDashboardService merchantDashboardService)
    {
        _merchantDashboardService = merchantDashboardService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(MerchantDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MerchantDashboardDto>> GetMyDashboardAsync(CancellationToken ct)
    {
        var result = await _merchantDashboardService.GetMyDashboardAsync(ct);
        return Ok(result);
    }
}
