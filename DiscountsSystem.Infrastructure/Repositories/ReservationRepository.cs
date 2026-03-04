using DiscountsSystem.Application.DTOs.Reservations.Results;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Repositories;

public sealed class ReservationRepository : IReservationRepository
{
    private readonly DiscountsDbContext _db;

    public ReservationRepository(DiscountsDbContext db) => _db = db;

    public async Task<(ReserveResult Result, Reservation? Reservation)> ReserveAsync(
        string customerId,
        int offerId,
        int quantity,
        DateTime nowUtc,
        DateTime expiresAtUtc,
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
            return (ReserveResult.OfferNotFound, null);

        if (!pre.IsActive || pre.Status != OfferStatus.Approved)
            return (ReserveResult.OfferNotReservable, null);

        if (pre.StartDateUtc > nowUtc || pre.EndDateUtc <= nowUtc)
            return (ReserveResult.OfferNotReservable, null);

        if (expiresAtUtc > pre.EndDateUtc)
            return (ReserveResult.OfferNotReservable, null);

        if (pre.CouponQuantityAvailable < quantity)
            return (ReserveResult.NotEnoughCoupons, null);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var offer = await _db.Offers.FirstOrDefaultAsync(o => o.Id == offerId, ct);
            if (offer is null)
            {
                await tx.RollbackAsync(ct);
                return (ReserveResult.OfferNotFound, null);
            }

            if (!offer.IsActive || offer.Status != OfferStatus.Approved)
            {
                await tx.RollbackAsync(ct);
                return (ReserveResult.OfferNotReservable, null);
            }

            if (offer.StartDateUtc > nowUtc || offer.EndDateUtc <= nowUtc)
            {
                await tx.RollbackAsync(ct);
                return (ReserveResult.OfferNotReservable, null);
            }

            if (expiresAtUtc > offer.EndDateUtc)
            {
                await tx.RollbackAsync(ct);
                return (ReserveResult.OfferNotReservable, null);
            }

            if (offer.CouponQuantityAvailable < quantity)
            {
                await tx.RollbackAsync(ct);
                return (ReserveResult.NotEnoughCoupons, null);
            }

            offer.CouponQuantityAvailable -= quantity;
            offer.UpdatedAtUtc = nowUtc;

            var reservation = new Reservation
            {
                OfferId = offerId,
                CustomerId = customerId,
                Quantity = quantity,
                Status = ReservationStatus.Active,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc,
                ExpiresAtUtc = expiresAtUtc
            };

            _db.Reservations.Add(reservation);

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return (ReserveResult.Success, reservation);
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return (ReserveResult.ConcurrencyConflict, null);
        }
        catch (DbUpdateException ex)
        {
            await tx.RollbackAsync(ct);

            if (IsUniqueViolation(ex))
                return (ReserveResult.AlreadyHasActiveReservation, null);

            return (ReserveResult.UnexpectedError, null);
        }
    }

    public async Task<bool> CancelAsync(
        int reservationId,
        string customerId,
        DateTime nowUtc,
        CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId, ct);
            if (reservation is null) return false;

            if (reservation.CustomerId != customerId) return false;
            if (reservation.Status != ReservationStatus.Active) return false;
            if (reservation.ExpiresAtUtc <= nowUtc) return false;

            var offer = await _db.Offers.FirstOrDefaultAsync(o => o.Id == reservation.OfferId, ct);
            if (offer is null) return false;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.UpdatedAtUtc = nowUtc;

            offer.CouponQuantityAvailable += reservation.Quantity;
            offer.UpdatedAtUtc = nowUtc;

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return false;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            return false;
        }
    }

    public async Task<int> ExpireDueAsync(DateTime nowUtc, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var due = await _db.Reservations
                .Where(r => r.Status == ReservationStatus.Active && r.ExpiresAtUtc <= nowUtc)
                .ToListAsync(ct);

            if (due.Count == 0) return 0;

            var offerIds = due.Select(r => r.OfferId).Distinct().ToList();
            var offers = await _db.Offers.Where(o => offerIds.Contains(o.Id)).ToListAsync(ct);

            var offersById = offers.ToDictionary(o => o.Id);

            foreach (var r in due)
            {
                r.Status = ReservationStatus.Expired;
                r.UpdatedAtUtc = nowUtc;

                if (offersById.TryGetValue(r.OfferId, out var offer))
                {
                    offer.CouponQuantityAvailable += r.Quantity;
                    offer.UpdatedAtUtc = nowUtc;
                }
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return due.Count;
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            return 0;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            return 0;
        }
    }

    public Task<Reservation?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<List<Reservation>> GetByCustomerAsync(string customerId, ReservationStatus? status, CancellationToken ct = default)
    {
        var q = _db.Reservations.AsNoTracking().Where(r => r.CustomerId == customerId);

        if (status is not null)
            q = q.Where(r => r.Status == status.Value);

        return q.OrderByDescending(r => r.CreatedAtUtc).ToListAsync(ct);
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx)
            return sqlEx.Number is 2601 or 2627;

        return false;
    }
}
