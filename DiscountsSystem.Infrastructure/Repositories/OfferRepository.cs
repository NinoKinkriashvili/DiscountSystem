using DiscountsSystem.Application.DTOs.MerchantDashboard;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Repositories;

public sealed class OfferRepository : IOfferRepository
{
    private readonly DiscountsDbContext _db;

    public OfferRepository(DiscountsDbContext db)
    {
        _db = db;
    }

    public Task<List<Offer>> GetPublicVisibleAsync(DateTime nowUtc, CancellationToken ct = default)
        => _db.Offers
            .AsNoTracking()
            .Where(o => o.IsActive
                        && o.StartDateUtc <= nowUtc
                        && o.EndDateUtc > nowUtc
                        && (
                            o.Status == OfferStatus.Approved
                            || (o.Status == OfferStatus.Pending && o.ApprovedAtUtc != null) // update-pending visible
                        ))
            .OrderBy(o => o.EndDateUtc)
            .ToListAsync(ct);

    public Task<Offer?> GetPublicVisibleByIdAsync(int id, DateTime nowUtc, CancellationToken ct = default)
        => _db.Offers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id
                                     && o.IsActive
                                     && o.StartDateUtc <= nowUtc
                                     && o.EndDateUtc > nowUtc
                                     && (
                                         o.Status == OfferStatus.Approved
                                         || (o.Status == OfferStatus.Pending && o.ApprovedAtUtc != null)
                                     ), ct);

    public Task<Offer?> GetByIdForUpdateAsync(int id, CancellationToken ct = default)
        => _db.Offers.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<List<Offer>> GetByMerchantAsync(string merchantId, CancellationToken ct = default)
        => _db.Offers
            .AsNoTracking()
            .Where(o => o.MerchantId == merchantId)
            .OrderByDescending(o => o.UpdatedAtUtc)
            .ToListAsync(ct);

    public Task<List<Offer>> GetPendingForModerationAsync(CancellationToken ct = default)
        => _db.Offers
            .AsNoTracking()
            .Where(o => o.Status == OfferStatus.Pending)
            .OrderByDescending(o => o.ApprovedAtUtc == null)
            .ThenByDescending(o => o.UpdatedAtUtc)
            .ToListAsync(ct);

    public async Task<MerchantDashboardOfferStatsDto> GetMerchantDashboardOfferStatsAsync(
        string merchantId,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        var rows = await _db.Offers
            .AsNoTracking()
            .Where(o => o.MerchantId == merchantId)
            .Select(o => new
            {
                o.Status,
                o.IsActive,
                o.EndDateUtc
            })
            .ToListAsync(ct);

        var totalOffers = rows.Count;

        var activeOffersCount = rows.Count(x =>
            x.IsActive &&
            x.Status == OfferStatus.Approved &&
            x.EndDateUtc > nowUtc);

        var expiredOffersCount = rows.Count(x =>
            x.IsActive &&
            x.Status == OfferStatus.Approved &&
            x.EndDateUtc <= nowUtc);

        var pendingOffersCount = rows.Count(x => x.Status == OfferStatus.Pending);
        var approvedOffersCount = rows.Count(x => x.Status == OfferStatus.Approved);
        var rejectedOffersCount = rows.Count(x => x.Status == OfferStatus.Rejected);

        var disabledOffersCount = rows.Count(x => !x.IsActive);

        var offerStatusBreakdown = rows
            .GroupBy(x => x.Status)
            .Select(g => new MerchantOfferStatusCountDto(
                g.Key,
                g.Count()))
            .OrderBy(x => x.Status)
            .ToList();

        return new MerchantDashboardOfferStatsDto(
            totalOffers,
            activeOffersCount,
            expiredOffersCount,
            pendingOffersCount,
            approvedOffersCount,
            rejectedOffersCount,
            disabledOffersCount,
            offerStatusBreakdown
        );
    }

    public async Task AddAsync(Offer offer, CancellationToken ct = default)
    {
        await _db.Offers.AddAsync(offer, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Offer offer, CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var offer = await _db.Offers.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (offer is null) return false;

        _db.Offers.Remove(offer);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> ExpireDueAsync(DateTime nowUtc, CancellationToken ct = default)
    {
        var dueOffers = await _db.Offers
            .Where(o =>
                o.IsActive &&
                o.EndDateUtc <= nowUtc &&
                (
                    o.Status == OfferStatus.Approved ||
                    (o.Status == OfferStatus.Pending && o.ApprovedAtUtc != null)
                ))
            .ToListAsync(ct);

        if (dueOffers.Count == 0)
            return 0;

        foreach (var offer in dueOffers)
        {
            offer.Status = OfferStatus.Expired;
            offer.UpdatedAtUtc = nowUtc;
        }

        await _db.SaveChangesAsync(ct);
        return dueOffers.Count;
    }
}
