using Asp.Versioning;
using DiscountsSystem.Application.DTOs.Reservations;
using DiscountsSystem.Application.DTOs.Reservations.Results;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Customer;

[ApiController]
[Authorize(Roles = "Customer")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservations;

    public ReservationsController(IReservationService reservations)
    {
        _reservations = reservations;
    }

    // Customer actions

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Reserve([FromBody] CreateReservationRequest request, CancellationToken ct)
    {
        var response = await _reservations.ReserveAsync(request, ct);

        return response.Result switch
        {
            ReserveResult.Success =>
                Created($"/api/v1/customer/reservations/{response.ReservationId}", new { id = response.ReservationId }),

            ReserveResult.OfferNotFound =>
                Problem(statusCode: StatusCodes.Status404NotFound, title: "Offer not found."),

            ReserveResult.OfferNotReservable =>
                Problem(statusCode: StatusCodes.Status400BadRequest, title: "Offer is not reservable."),

            ReserveResult.NotEnoughCoupons =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Not enough coupons available."),

            ReserveResult.AlreadyHasActiveReservation =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "You already have an active reservation for this offer."),

            ReserveResult.ConcurrencyConflict =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Concurrency conflict. Please try again."),

            _ =>
                Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error.")
        };
    }

    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel([FromBody] CancelReservationRequest request, CancellationToken ct)
    {
        var ok = await _reservations.CancelAsync(request.ReservationId, ct);
        return ok ? NoContent() : NotFound();
    }

    // Customer reads

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await _reservations.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy([FromQuery] ReservationStatus? status, CancellationToken ct)
    {
        var list = await _reservations.GetMyAsync(status, ct);
        return Ok(list);
    }
}
