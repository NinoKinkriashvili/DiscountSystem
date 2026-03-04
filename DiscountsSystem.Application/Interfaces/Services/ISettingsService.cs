using DiscountsSystem.Application.DTOs.Settings;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface ISettingsService
{
    Task<SettingsDto> GetAsync(CancellationToken ct = default);
    Task<SettingsDto> UpdateAsync(UpdateSettingsRequest request, CancellationToken ct = default);
}