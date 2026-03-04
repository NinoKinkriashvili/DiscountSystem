using DiscountsSystem.Application.DTOs.Reservations;
using DiscountsSystem.Application.DTOs.Reservations.Results;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Services.Reservations;

public sealed class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservations;
    private readonly ISettingsRepository _settings;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _time;

    public ReservationService(
        IReservationRepository reservations,
        ISettingsRepository settings,
        ICurrentUserService currentUser,
        IDateTimeProvider time)
    {
        _reservations = reservations;
        _settings = settings;
        _currentUser = currentUser;
        _time = time;
    }

    // Customer

    public async Task<ReserveResponseDto> ReserveAsync(CreateReservationRequest request, CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var now = _time.UtcNow;

        var settings = await _settings.GetCurrentAsync(ct);
        if (settings is null)
            throw new InvalidOperationException("Settings not found.");

        var expiresAt = now.AddMinutes(settings.ReservationDurationMinutes);

        var (result, reservation) = await _reservations.ReserveAsync(
            customerId,
            request.OfferId,
            request.Quantity,
            now,
            expiresAt,
            ct);

        return new ReserveResponseDto(result, reservation?.Id);
    }

    public Task<bool> CancelAsync(int reservationId, CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var now = _time.UtcNow;

        return _reservations.CancelAsync(reservationId, customerId, now, ct);
    }

    // Customer

    public async Task<ReservationDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;

        var r = await _reservations.GetByIdAsync(id, ct);
        if (r is null) return null;

        if (r.CustomerId != customerId)
            throw new UnauthorizedAccessException("You can view only your own reservations.");

        return Map(r);
    }

    public async Task<List<ReservationDto>> GetMyAsync(ReservationStatus? status, CancellationToken ct = default)
    {
        EnsureCustomer();

        var customerId = _currentUser.UserId!;
        var list = await _reservations.GetByCustomerAsync(customerId, status, ct);

        return list.Select(Map).ToList();
    }

    // Worker (expiration)

    public Task<int> ExpireDueAsync(CancellationToken ct = default)
    {
        var now = _time.UtcNow;
        return _reservations.ExpireDueAsync(now, ct);
    }

    // Helpers

    private void EnsureCustomer()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (_currentUser.Role is not UserRole.Customer)
            throw new UnauthorizedAccessException("Only Customer can perform this action.");
    }

    private static ReservationDto Map(Domain.Entities.Reservation r)
        => new(
            r.Id,
            r.OfferId,
            r.Quantity,
            r.Status,
            r.CreatedAtUtc,
            r.UpdatedAtUtc,
            r.ExpiresAtUtc
        );
}
