using DiscountsSystem.Application.DTOs.Offers;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Services.Offers;

public sealed class OfferService : IOfferService
{
    private readonly IOfferRepository _offers;
    private readonly ICategoryRepository _categories;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _time;
    private readonly ISettingsRepository _settings;
    private const int SettingsId = 1;

    public OfferService(
        IOfferRepository offers,
        ICategoryRepository categories,
        ICurrentUserService currentUser,
        IDateTimeProvider time,
        ISettingsRepository settings)
    {
        _offers = offers;
        _categories = categories;
        _currentUser = currentUser;
        _time = time;
        _settings = settings;
    }

    // Public
    public async Task<List<OfferListItemDto>> GetPublicActiveAsync(CancellationToken ct = default)
    {
        var now = _time.UtcNow;
        var offers = await _offers.GetPublicVisibleAsync(now, ct);
        return offers.Select(MapListItem).ToList();
    }

    public async Task<OfferPublicDetailsDto?> GetPublicByIdAsync(int id, CancellationToken ct = default)
    {
        var now = _time.UtcNow;
        var offer = await _offers.GetPublicVisibleByIdAsync(id, now, ct);
        if (offer is null) return null;

        return MapPublicDetails(offer, isActionBlocked: IsActionBlockedForUpdatePending(offer));
    }

    // Merchant
    public async Task<List<OfferListItemDto>> GetMyAsync(CancellationToken ct = default)
    {
        EnsureRole(UserRole.Merchant);

        var offers = await _offers.GetByMerchantAsync(_currentUser.UserId!, ct);
        return offers.Select(MapListItem).ToList();
    }

    public async Task<int> CreateAsync(CreateOfferRequest request, CancellationToken ct = default)
    {
        EnsureRole(UserRole.Merchant);

        await GetActiveCategoryOrThrowAsync(request.CategoryId, ct);

        var now = _time.UtcNow;

        var offer = new Offer
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            OriginalPrice = request.OriginalPrice,
            DiscountPrice = request.DiscountPrice,

            CouponQuantityTotal = request.CouponQuantityTotal,
            CouponQuantityAvailable = request.CouponQuantityTotal,

            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc,

            CategoryId = request.CategoryId,

            Status = OfferStatus.Pending,
            RejectReason = null,

            IsActive = true,

            MerchantId = _currentUser.UserId!,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,

            ApprovedAtUtc = null
        };

        await _offers.AddAsync(offer, ct);
        return offer.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateOfferRequest request, CancellationToken ct = default)
    {
        EnsureRoleAny(UserRole.Merchant, UserRole.Administrator);

        var offer = await _offers.GetByIdForUpdateAsync(id, ct);
        if (offer is null) return false;

        EnsureMerchantOwnsOfferIfMerchant(offer);

        // Merchant edit window rule
        if (_currentUser.Role == UserRole.Merchant)
            await EnsureMerchantEditWindowOrThrowAsync(offer, ct);

        EnsureEditableStatus(offer);
        await GetActiveCategoryOrThrowAsync(request.CategoryId, ct);

        var now = _time.UtcNow;

        offer.Title = request.Title.Trim();
        offer.Description = request.Description.Trim();
        offer.OriginalPrice = request.OriginalPrice;
        offer.DiscountPrice = request.DiscountPrice;
        offer.StartDateUtc = request.StartDateUtc;
        offer.EndDateUtc = request.EndDateUtc;
        offer.CategoryId = request.CategoryId;

        var lockedOrConsumed = offer.CouponQuantityTotal - offer.CouponQuantityAvailable;

        if (request.CouponQuantityTotal < lockedOrConsumed)
        {
            throw new InvalidOperationException(
                $"CouponQuantityTotal cannot be less than already reserved/purchased amount ({lockedOrConsumed}).");
        }

        offer.CouponQuantityTotal = request.CouponQuantityTotal;
        offer.CouponQuantityAvailable = request.CouponQuantityTotal - lockedOrConsumed;

        // Approved -> Pending again (update pending, still visible read-only on public)
        if (offer.Status == OfferStatus.Approved)
        {
            offer.Status = OfferStatus.Pending;
            offer.RejectReason = null;

        }

        offer.UpdatedAtUtc = now;

        await _offers.UpdateAsync(offer, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        EnsureRoleAny(UserRole.Merchant, UserRole.Administrator);

        var offer = await _offers.GetByIdForUpdateAsync(id, ct);
        if (offer is null) return false;

        EnsureMerchantOwnsOfferIfMerchant(offer);

        // Only Pending can be hard-deleted
        if (offer.Status == OfferStatus.Pending)
            return await _offers.DeleteAsync(id, ct);

        // Otherwise -> soft disable
        offer.IsActive = false;
        offer.Status = OfferStatus.Disabled;
        offer.UpdatedAtUtc = _time.UtcNow;

        await _offers.UpdateAsync(offer, ct);
        return true;
    }

    // Admin
    public async Task<List<OfferListItemDto>> GetPendingForModerationAsync(CancellationToken ct = default)
    {
        EnsureRole(UserRole.Administrator);

        var offers = await _offers.GetPendingForModerationAsync(ct);
        return offers.Select(MapListItem).ToList();
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateOfferStatusRequest request, CancellationToken ct = default)
    {
        EnsureRole(UserRole.Administrator);

        var offer = await _offers.GetByIdForUpdateAsync(id, ct);
        if (offer is null) return false;

        if (offer.Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be moderated.");

        if (!Enum.TryParse<OfferStatus>(request.Status, ignoreCase: true, out var parsedStatus))
            throw new InvalidOperationException("Status must be Approved or Rejected.");

        if (parsedStatus is not (OfferStatus.Approved or OfferStatus.Rejected))
            throw new InvalidOperationException("Status must be Approved or Rejected.");

        var now = _time.UtcNow;

        if (parsedStatus == OfferStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(request.RejectReason))
                throw new InvalidOperationException("Reject reason is required.");

            offer.Status = OfferStatus.Rejected;
            offer.RejectReason = request.RejectReason.Trim();
        }
        else
        {
            offer.Status = OfferStatus.Approved;
            offer.RejectReason = null;
            offer.ApprovedAtUtc = now;
        }

        offer.UpdatedAtUtc = now;

        await _offers.UpdateAsync(offer, ct);
        return true;
    }

    // Helpers
    private static OfferListItemDto MapListItem(Offer o) => new(
        o.Id,
        o.Title,
        o.DiscountPrice,
        o.EndDateUtc,
        o.CouponQuantityAvailable,
        o.Status,
        o.CategoryId
    );

    private static OfferPublicDetailsDto MapPublicDetails(Offer o, bool isActionBlocked) => new(
        o.Id,
        o.Title,
        o.Description,
        o.OriginalPrice,
        o.DiscountPrice,
        o.CouponQuantityTotal,
        o.CouponQuantityAvailable,
        o.StartDateUtc,
        o.EndDateUtc,
        o.Status,
        o.CategoryId,
        isActionBlocked
    );

    private static bool IsActionBlockedForUpdatePending(Offer offer)
        => offer.Status == OfferStatus.Pending && offer.ApprovedAtUtc != null;

    private async Task<Category> GetActiveCategoryOrThrowAsync(int categoryId, CancellationToken ct)
    {
        var category = await _categories.GetActiveByIdAsync(categoryId, ct);
        if (category is null)
            throw new InvalidOperationException("Category not found or inactive.");

        return category;
    }

    private void EnsureEditableStatus(Offer offer)
    {
        if (offer.Status is OfferStatus.Disabled or OfferStatus.Expired)
            throw new InvalidOperationException($"Cannot edit offer when status is {offer.Status}.");
    }

    private void EnsureMerchantOwnsOfferIfMerchant(Offer offer)
    {
        if (_currentUser.Role != UserRole.Merchant)
            return;

        var myId = _currentUser.UserId!;
        if (offer.MerchantId != myId)
            throw new UnauthorizedAccessException("You can access only your own offers.");
    }

    private void EnsureAuthenticated()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");
    }

    private void EnsureRole(UserRole requiredRole)
    {
        EnsureAuthenticated();
        if (_currentUser.Role != requiredRole)
            throw new UnauthorizedAccessException($"Only {requiredRole} can perform this action.");
    }

    private void EnsureRoleAny(params UserRole[] roles)
    {
        EnsureAuthenticated();
        if (Array.IndexOf(roles, _currentUser.Role) < 0)
            throw new UnauthorizedAccessException($"Only {string.Join(", ", roles)} can perform this action.");
    }


    private async Task EnsureMerchantEditWindowOrThrowAsync(Offer offer, CancellationToken ct)
    {
        if (offer.Status == OfferStatus.Pending && offer.ApprovedAtUtc == null)
            return;

        if (offer.ApprovedAtUtc is null)
            throw new InvalidOperationException("Offer is not approved yet.");

        var settings = await _settings.GetByIdAsync(SettingsId, ct);
        if (settings is null)
            throw new InvalidOperationException("Settings not found.");

        var deadline = offer.ApprovedAtUtc.Value.AddHours(settings.MerchantEditWindowHours);

        if (_time.UtcNow > deadline)
            throw new InvalidOperationException("Edit window has expired.");
    }
}
