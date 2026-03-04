using DiscountsSystem.Application.DTOs.MerchantSales;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Services.Merchant;

public sealed class MerchantSalesHistoryService : IMerchantSalesHistoryService
{
    private readonly IPurchaseRepository _purchases;
    private readonly ICurrentUserService _currentUser;

    public MerchantSalesHistoryService(
        IPurchaseRepository purchases,
        ICurrentUserService currentUser)
    {
        _purchases = purchases;
        _currentUser = currentUser;
    }

    public Task<List<MerchantSaleListItemDto>> GetMySalesHistoryAsync(CancellationToken ct = default)
    {
        EnsureMerchant();

        var merchantId = _currentUser.UserId!;

        return _purchases.GetMerchantSalesHistoryAsync(
            merchantId: merchantId,
            ct: ct);
    }

    private void EnsureMerchant()
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated.");

        if (_currentUser.Role is not UserRole.Merchant)
            throw new UnauthorizedAccessException("Only Merchant can perform this action.");
    }
}