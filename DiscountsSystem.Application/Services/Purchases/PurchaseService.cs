using DiscountsSystem.Application.DTOs.MyCoupons;
using DiscountsSystem.Application.DTOs.Purchases;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Services.Purchases;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchases;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _time;

    public PurchaseService(
        IPurchaseRepository purchases,
        ICurrentUserService currentUser,
        IDateTimeProvider time)
    {
        _purchases = purchases;
        _currentUser = currentUser;
        _time = time;
    }
    // Customer - purchase actions

    public async Task<PurchaseResponseDto> CreateDirectAsync(
        CreateDirectPurchaseRequest request,
        CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var now = _time.UtcNow;

        var (result, purchase) = await _purchases.CreateDirectAsync(
            customerId,
            request.OfferId,
            request.Quantity,
            now,
            ct);

        return new PurchaseResponseDto(result, purchase?.Id);
    }

    public async Task<PurchaseResponseDto> CreateFromReservationAsync(
        CreatePurchaseFromReservationRequest request,
        CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var now = _time.UtcNow;

        var (result, purchase) = await _purchases.CreateFromReservationAsync(
            customerId,
            request.ReservationId,
            now,
            ct);

        return new PurchaseResponseDto(result, purchase?.Id);
    }

    // Customer - My Coupons

    public async Task<MyCouponDetailsDto?> GetMyCouponByIdAsync(int purchaseId, CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;

        var purchase = await _purchases.GetByIdAsync(purchaseId, ct);
        if (purchase is null)
            return null;

        if (purchase.CustomerId != customerId)
            throw new UnauthorizedAccessException("You can view only your own coupons.");

        return MapToDetails(purchase);
    }

    public async Task<List<MyCouponListItemDto>> GetMyCouponsAsync(
        CouponPurchaseStatus? status,
        CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var list = await _purchases.GetByCustomerAsync(customerId, status, ct);

        return list.Select(MapToListItem).ToList();
    }

    // Worker - coupon expiration

    public Task<int> ExpireDueCouponsAsync(CancellationToken ct = default)
    {
        var now = _time.UtcNow;
        return _purchases.ExpireDueCouponsAsync(now, ct);
    }

    // Helpers

    private void EnsureCustomer()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (_currentUser.Role is not UserRole.Customer)
            throw new UnauthorizedAccessException("Only Customer can perform this action.");
    }

    private static MyCouponListItemDto MapToListItem(CouponPurchase p)
        => new(
            PurchaseId: p.Id,
            OfferId: p.OfferId,
            OfferTitle: p.Offer?.Title ?? string.Empty,
            Quantity: p.Quantity,
            CouponCode: p.CouponCode,
            Status: p.Status,
            PurchasedAtUtc: p.CreatedAtUtc,
            ExpiresAtUtc: p.ExpiresAtUtc
        );

    private static MyCouponDetailsDto MapToDetails(CouponPurchase p)
        => new(
            PurchaseId: p.Id,
            OfferId: p.OfferId,
            OfferTitle: p.Offer?.Title ?? string.Empty,
            OfferDescription: p.Offer?.Description,
            Quantity: p.Quantity,
            CouponCode: p.CouponCode,
            Status: p.Status,
            PurchasedAtUtc: p.CreatedAtUtc,
            ExpiresAtUtc: p.ExpiresAtUtc,
            ReservationId: p.ReservationId,
            SourceType: p.SourceType
        );
}
