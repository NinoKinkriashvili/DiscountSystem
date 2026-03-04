using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Offers;
using DiscountsSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Public;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/public/offers")]
public sealed class PublicOffersController : ControllerBase
{
    private readonly IOfferService _offers;

    public PublicOffersController(IOfferService offers)
    {
        _offers = offers;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OfferListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicVisible(CancellationToken ct)
    {
        var result = await _offers.GetPublicActiveAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OfferPublicDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicById(int id, CancellationToken ct)
    {
        var offer = await _offers.GetPublicByIdAsync(id, ct);
        return offer is null ? NotFound() : Ok(offer);
    }
}
