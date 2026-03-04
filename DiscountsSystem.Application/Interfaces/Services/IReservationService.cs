using DiscountsSystem.Application.DTOs.Reservations;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface IReservationService
{
    Task<ReserveResponseDto> ReserveAsync(CreateReservationRequest request, CancellationToken ct = default);
    Task<bool> CancelAsync(int reservationId, CancellationToken ct = default);

    Task<ReservationDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<ReservationDto>> GetMyAsync(ReservationStatus? status, CancellationToken ct = default);
    Task<int> ExpireDueAsync(CancellationToken ct = default);
}
