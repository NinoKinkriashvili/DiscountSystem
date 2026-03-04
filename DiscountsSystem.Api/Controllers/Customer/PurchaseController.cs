using Asp.Versioning;
using DiscountsSystem.Application.DTOs.MyCoupons;
using DiscountsSystem.Application.DTOs.Purchases;
using DiscountsSystem.Application.DTOs.Purchases.Results;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscountsSystem.Api.Controllers.Customer;

[ApiController]
[Authorize(Roles = "Customer")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer/purchases")]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchases;

    public PurchasesController(IPurchaseService purchases)
    {
        _purchases = purchases;
    }


    // Direct purchase (without reservation)
    [HttpPost("direct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDirect([FromBody] CreateDirectPurchaseRequest request, CancellationToken ct)
    {
        var response = await _purchases.CreateDirectAsync(request, ct);

        return response.Result switch
        {
            PurchaseResult.Success =>
                Created($"/api/v1/customer/purchases/my-coupons/{response.PurchaseId}", new { id = response.PurchaseId }),

            PurchaseResult.OfferNotFound =>
                Problem(statusCode: StatusCodes.Status404NotFound, title: "Offer not found."),

            PurchaseResult.OfferNotPurchasable =>
                Problem(statusCode: StatusCodes.Status400BadRequest, title: "Offer is not purchasable."),

            PurchaseResult.NotEnoughCoupons =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Not enough coupons available."),

            PurchaseResult.ConcurrencyConflict =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Concurrency conflict. Please try again."),

            _ =>
                Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error.")
        };
    }

    // Purchase from existing reservation
    [HttpPost("from-reservation")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateFromReservation([FromBody] CreatePurchaseFromReservationRequest request, CancellationToken ct)
    {
        var response = await _purchases.CreateFromReservationAsync(request, ct);

        return response.Result switch
        {
            PurchaseResult.Success =>
                Created($"/api/v1/customer/purchases/my-coupons/{response.PurchaseId}", new { id = response.PurchaseId }),

            PurchaseResult.ReservationNotFound =>
                Problem(statusCode: StatusCodes.Status404NotFound, title: "Reservation not found."),


            PurchaseResult.ReservationNotOwnedByCustomer =>
                Problem(statusCode: StatusCodes.Status404NotFound, title: "Reservation not found."),

            PurchaseResult.ReservationNotActive =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Reservation is not active."),

            PurchaseResult.ReservationAlreadyPurchased =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "This reservation has already been purchased."),

            PurchaseResult.ReservationExpired =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Reservation is expired."),

            PurchaseResult.OfferNotFound =>
                Problem(statusCode: StatusCodes.Status404NotFound, title: "Offer not found."),

            PurchaseResult.OfferNotPurchasable =>
                Problem(statusCode: StatusCodes.Status400BadRequest, title: "Offer is not purchasable."),

            PurchaseResult.NotEnoughCoupons =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Not enough coupons available."),

            PurchaseResult.ConcurrencyConflict =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Concurrency conflict. Please try again."),

            PurchaseResult.ReservationOfferMismatch =>
                Problem(statusCode: StatusCodes.Status409Conflict, title: "Reservation/offer mismatch."),

            _ =>
                Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error.")
        };
    }

    // Customer - My coupons

    [HttpGet("my-coupons")]
    [ProducesResponseType(typeof(List<MyCouponListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCoupons([FromQuery] CouponPurchaseStatus? status, CancellationToken ct)
    {
        var list = await _purchases.GetMyCouponsAsync(status, ct);
        return Ok(list);
    }

    [HttpGet("my-coupons/{purchaseId:int}")]
    [ProducesResponseType(typeof(MyCouponDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCouponById(int purchaseId, CancellationToken ct)
    {
        var dto = await _purchases.GetMyCouponByIdAsync(purchaseId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
