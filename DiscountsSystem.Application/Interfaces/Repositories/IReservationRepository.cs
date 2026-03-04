using DiscountsSystem.Application.DTOs.Reservations.Results;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Repositories;

public interface IReservationRepository
{
    Task<(ReserveResult Result, Reservation? Reservation)> ReserveAsync(
        string customerId,
        int offerId,
        int quantity,
        DateTime nowUtc,
        DateTime expiresAtUtc,
        CancellationToken ct = default);

    Task<bool> CancelAsync(
        int reservationId,
        string customerId,
        DateTime nowUtc,
        CancellationToken ct = default);

    Task<int> ExpireDueAsync(DateTime nowUtc, CancellationToken ct = default);

    Task<Reservation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Reservation>> GetByCustomerAsync(string customerId, ReservationStatus? status, CancellationToken ct = default);
}
