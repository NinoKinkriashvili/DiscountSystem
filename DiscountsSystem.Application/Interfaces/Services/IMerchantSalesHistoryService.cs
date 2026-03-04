using DiscountsSystem.Application.DTOs.MerchantSales;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface IMerchantSalesHistoryService
{
    Task<List<MerchantSaleListItemDto>> GetMySalesHistoryAsync(CancellationToken ct = default);
}