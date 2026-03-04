using DiscountsSystem.Application.DTOs.Offers;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface IOfferService
{
    Task<List<OfferListItemDto>> GetPublicActiveAsync(CancellationToken ct = default);
    Task<OfferPublicDetailsDto?> GetPublicByIdAsync(int id, CancellationToken ct = default);

    Task<List<OfferListItemDto>> GetMyAsync(CancellationToken ct = default);
    Task<int> CreateAsync(CreateOfferRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateOfferRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<List<OfferListItemDto>> GetPendingForModerationAsync(CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(int id, UpdateOfferStatusRequest request, CancellationToken ct = default);
}
