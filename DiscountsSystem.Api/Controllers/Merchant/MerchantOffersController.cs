using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Common;
using DiscountsSystem.Application.DTOs.Offers;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Merchant;

[ApiController]
[Authorize(Roles = "Merchant")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/merchant/offers")]
public sealed class MerchantOffersController : ControllerBase
{
    private readonly IOfferService _offers;

    public MerchantOffersController(IOfferService offers)
    {
        _offers = offers;
    }

    // Merchant list
    [HttpGet]
    [ProducesResponseType(typeof(List<OfferListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var result = await _offers.GetMyAsync(ct);
        return Ok(result);
    }

    // Create offer - returns created id

    [HttpPost]
    [ProducesResponseType(typeof(CreatedIdDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateOfferRequest request, CancellationToken ct)
    {
        var id = await _offers.CreateAsync(request, ct);
        return Created($"/api/v1/merchant/offers/{id}", new CreatedIdDto(id));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferRequest request, CancellationToken ct)
    {
        var ok = await _offers.UpdateAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _offers.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
