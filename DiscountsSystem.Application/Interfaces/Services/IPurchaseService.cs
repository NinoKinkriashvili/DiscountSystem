using DiscountsSystem.Application.DTOs.MyCoupons;
using DiscountsSystem.Application.DTOs.Purchases;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface IPurchaseService
{
    Task<PurchaseResponseDto> CreateDirectAsync(CreateDirectPurchaseRequest request, CancellationToken ct = default);
    Task<PurchaseResponseDto> CreateFromReservationAsync(CreatePurchaseFromReservationRequest request, CancellationToken ct = default);

    Task<MyCouponDetailsDto?> GetMyCouponByIdAsync(int purchaseId, CancellationToken ct = default);
    Task<List<MyCouponListItemDto>> GetMyCouponsAsync(CouponPurchaseStatus? status, CancellationToken ct = default);
    Task<int> ExpireDueCouponsAsync(CancellationToken ct = default);
}
