using Asp.Versioning;
using DiscountsSystem.Application.DTOs.MerchantSales;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Merchant;

[ApiController]
[Authorize(Roles = "Merchant")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/merchant/sales")]
public sealed class MerchantSalesController : ControllerBase
{
    private readonly IMerchantSalesHistoryService _merchantSalesHistoryService;

    public MerchantSalesController(IMerchantSalesHistoryService merchantSalesHistoryService)
    {
        _merchantSalesHistoryService = merchantSalesHistoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MerchantSaleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MerchantSaleListItemDto>>> GetMySalesHistoryAsync(CancellationToken ct)
    {
        var result = await _merchantSalesHistoryService.GetMySalesHistoryAsync(ct);
        return Ok(result);
    }
}
