using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Offers;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Administrator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/offers")]
public sealed class AdminOffersController : ControllerBase
{
    private readonly IOfferService _offers;

    public AdminOffersController(IOfferService offers)
    {
        _offers = offers;
    }

    // Admin sees ALL pending: new + update-pending
    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<OfferListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var result = await _offers.GetPendingForModerationAsync(ct);
        return Ok(result);
    }

    // Approve / Reject
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOfferStatusRequest request, CancellationToken ct)
    {
        var ok = await _offers.UpdateStatusAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }
}
