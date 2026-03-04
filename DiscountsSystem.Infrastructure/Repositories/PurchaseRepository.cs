using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Application.DTOs.MerchantSales;
using DiscountsSystem.Application.DTOs.Purchases.Results;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Repositories;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly DiscountsDbContext _db;

    public PurchaseRepository(DiscountsDbContext db)
    {
        _db = db;
    }

    public async Task<(PurchaseResult Result, CouponPurchase? Purchase)> CreateDirectAsync(
        string customerId,
        int offerId,
        int quantity,
        DateTime nowUtc,
        CancellationToken ct = default)
    {

        var pre = await _db.Offers
            .AsNoTracking()
            .Select(o => new
            {
                o.Id,
                o.IsActive,
                o.Status,
                o.StartDateUtc,
                o.EndDateUtc,
                o.ApprovedAtUtc,
                o.CouponQuantityAvailable
            })
            .FirstOrDefaultAsync(o => o.Id == offerId, ct);

        if (pre is null)
            return (PurchaseResult.OfferNotFound, null);

        var isApproved = pre.Status == OfferStatus.Approved;
        var isUpdatePending = pre.Status == OfferStatus.Pending && pre.ApprovedAtUtc != null;

        if (!pre.IsActive || !isApproved || isUpdatePending)
            return (PurchaseResult.OfferNotPurchasable, null);

        if (pre.StartDateUtc > nowUtc || pre.EndDateUtc <= nowUtc)
            return (PurchaseResult.OfferNotPurchasable, null);

        if (pre.CouponQuantityAvailable < quantity)
            return (PurchaseResult.NotEnoughCoupons, null);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var offer = await _db.Offers.FirstOrDefaultAsync(o => o.Id == offerId, ct);
            if (offer is null)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotFound, null);
            }

            var offerIsApproved = offer.Status == OfferStatus.Approved;
            var offerIsUpdatePending = offer.Status == OfferStatus.Pending && offer.ApprovedAtUtc != null;

            if (!offer.IsActive || !offerIsApproved || offerIsUpdatePending)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotPurchasable, null);
            }

            if (offer.StartDateUtc > nowUtc || offer.EndDateUtc <= nowUtc)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotPurchasable, null);
            }

            if (offer.CouponQuantityAvailable < quantity)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.NotEnoughCoupons, null);
            }

            offer.CouponQuantityAvailable -= quantity;
            offer.UpdatedAtUtc = nowUtc;

            var purchase = new CouponPurchase
            {
                CustomerId = customerId,
                OfferId = offer.Id,
                Quantity = quantity,
                ReservationId = null,
                SourceType = PurchaseSourceType.Direct,
                Status = CouponPurchaseStatus.Active,
                CouponCode = GenerateCouponCode(),
                ExpiresAtUtc = offer.EndDateUtc, // snapshot
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            _db.CouponPurchases.Add(purchase);

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return (PurchaseResult.Success, purchase);
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.ConcurrencyConflict, null);
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.UnexpectedError, null);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.UnexpectedError, null);
        }
    }

    public async Task<(PurchaseResult Result, CouponPurchase? Purchase)> CreateFromReservationAsync(
        string customerId,
        int reservationId,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId, ct);

            if (reservation is null)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.ReservationNotFound, null);
            }

            if (reservation.CustomerId != customerId)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.ReservationNotOwnedByCustomer, null);
            }

            if (reservation.Status != ReservationStatus.Active)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.ReservationNotActive, null);
            }

            if (reservation.ExpiresAtUtc <= nowUtc)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.ReservationExpired, null);
            }

            var offer = await _db.Offers.FirstOrDefaultAsync(o => o.Id == reservation.OfferId, ct);
            if (offer is null)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotFound, null);
            }

            var offerIsApproved = offer.Status == OfferStatus.Approved;
            var offerIsUpdatePending = offer.Status == OfferStatus.Pending && offer.ApprovedAtUtc != null;

            if (!offer.IsActive || !offerIsApproved || offerIsUpdatePending)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotPurchasable, null);
            }

            if (offer.StartDateUtc > nowUtc || offer.EndDateUtc <= nowUtc)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.OfferNotPurchasable, null);
            }

            var alreadyPurchased = await _db.CouponPurchases
                .AsNoTracking()
                .AnyAsync(p =>
                    p.ReservationId == reservation.Id &&
                    p.SourceType == PurchaseSourceType.FromReservation, ct);

            if (alreadyPurchased)
            {
                await tx.RollbackAsync(ct);
                return (PurchaseResult.ReservationAlreadyPurchased, null);
            }

            reservation.Status = ReservationStatus.Purchased;
            reservation.UpdatedAtUtc = nowUtc;

            var purchase = new CouponPurchase
            {
                CustomerId = customerId,
                OfferId = reservation.OfferId,
                Quantity = reservation.Quantity,
                ReservationId = reservation.Id,
                SourceType = PurchaseSourceType.FromReservation,
                Status = CouponPurchaseStatus.Active,
                CouponCode = GenerateCouponCode(),
                ExpiresAtUtc = offer.EndDateUtc, // snapshot
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            _db.CouponPurchases.Add(purchase);

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return (PurchaseResult.Success, purchase);
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.ConcurrencyConflict, null);
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.UnexpectedError, null);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            return (PurchaseResult.UnexpectedError, null);
        }
    }
    public async Task<int> ExpireDueCouponsAsync(DateTime nowUtc, CancellationToken ct = default)
    {
        try
        {
            var affectedRows = await _db.CouponPurchases
                .Where(p => p.Status == CouponPurchaseStatus.Active && p.ExpiresAtUtc <= nowUtc)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, CouponPurchaseStatus.Expired)
                    .SetProperty(p => p.UpdatedAtUtc, nowUtc), ct);

            return affectedRows;
        }
        catch (DbUpdateConcurrencyException)
        {
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    public Task<CouponPurchase?> GetByIdAsync(int purchaseId, CancellationToken ct = default)
        => _db.CouponPurchases
            .AsNoTracking()
            .Include(p => p.Offer)
            .FirstOrDefaultAsync(p => p.Id == purchaseId, ct);

    public Task<List<CouponPurchase>> GetByCustomerAsync(
        string customerId,
        CouponPurchaseStatus? status,
        CancellationToken ct = default)
    {
        var q = _db.CouponPurchases
            .AsNoTracking()
            .Include(p => p.Offer)
            .Where(p => p.CustomerId == customerId);

        if (status is not null)
            q = q.Where(p => p.Status == status.Value);

        return q
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public Task<List<MerchantSaleListItemDto>> GetMerchantSalesHistoryAsync(
        string merchantId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        int? offerId = null,
        CancellationToken ct = default)
    {
        var q = _db.CouponPurchases
            .AsNoTracking()
            .Include(p => p.Offer)
            .Where(p => p.Offer != null && p.Offer.MerchantId == merchantId);

        if (fromUtc is not null)
            q = q.Where(p => p.CreatedAtUtc >= fromUtc.Value);

        if (toUtc is not null)
            q = q.Where(p => p.CreatedAtUtc <= toUtc.Value);

        if (offerId is not null)
            q = q.Where(p => p.OfferId == offerId.Value);

        return q
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new MerchantSaleListItemDto(
                p.Id,
                p.OfferId,
                p.Offer != null ? p.Offer.Title : string.Empty,
                p.CustomerId,
                p.Quantity,
                p.CouponCode,
                p.CreatedAtUtc,
                p.ExpiresAtUtc,
                p.Status,
                p.SourceType,
                p.ReservationId
            ))
            .ToListAsync(ct);
    }

    public async Task<MerchantDashboardPurchaseStatsDto> GetMerchantDashboardPurchaseStatsAsync(
        string merchantId,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        var baseQuery = _db.CouponPurchases
            .AsNoTracking()
            .Include(p => p.Offer)
            .Where(p => p.Offer != null && p.Offer.MerchantId == merchantId);

        var rows = await baseQuery
            .Select(p => new
            {
                p.Status,
                p.Quantity
            })
            .ToListAsync(ct);

        var totalPurchasesCount = rows.Count;
        var totalSoldCouponsCount = rows.Sum(x => x.Quantity);

        var activePurchasedCouponsCount = rows
            .Where(x => x.Status == CouponPurchaseStatus.Active)
            .Sum(x => x.Quantity);

        var expiredPurchasedCouponsCount = rows
            .Where(x => x.Status == CouponPurchaseStatus.Expired)
            .Sum(x => x.Quantity);

        var purchaseStatusBreakdown = rows
            .GroupBy(x => x.Status)
            .Select(g => new MerchantPurchaseStatusCountDto(
                g.Key,
                g.Sum(x => x.Quantity)))
            .OrderBy(x => x.Status)
            .ToList();

        return new MerchantDashboardPurchaseStatsDto(
            TotalPurchasesCount: totalPurchasesCount,
            TotalSoldCouponsCount: totalSoldCouponsCount,
            ActivePurchasedCouponsCount: activePurchasedCouponsCount,
            ExpiredPurchasedCouponsCount: expiredPurchasedCouponsCount,
            PurchaseStatusBreakdown: purchaseStatusBreakdown
        );
    }

    private static string GenerateCouponCode()
    {
        var part = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        return $"CPN-{part}";
    }
}
