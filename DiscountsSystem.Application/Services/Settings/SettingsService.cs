using DiscountsSystem.Application.DTOs.Settings;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Exceptions;

namespace DiscountsSystem.Application.Services.Settings;

public sealed class SettingsService : ISettingsService
{
    private const int SettingsId = 1;
    private readonly ISettingsRepository _repo;

    public SettingsService(ISettingsRepository repo)
    {
        _repo = repo;
    }

    public async Task<SettingsDto> GetAsync(CancellationToken ct = default)
    {
        var settings = await _repo.GetByIdAsync(SettingsId, ct);
        if (settings is null)
            throw new NotFoundException("Settings not found. Seed should create it.");

        return Map(settings);
    }

    public async Task<SettingsDto> UpdateAsync(UpdateSettingsRequest request, CancellationToken ct = default)
    {
        var settings = await _repo.GetByIdAsync(SettingsId, ct);
        if (settings is null)
            throw new NotFoundException("Settings not found.");

        settings.ReservationDurationMinutes = request.ReservationDurationMinutes;
        settings.MerchantEditWindowHours = request.MerchantEditWindowHours;

        await _repo.SaveChangesAsync(ct);

        return Map(settings);
    }

    private static SettingsDto Map(Domain.Entities.Settings s)
        => new SettingsDto(s.ReservationDurationMinutes, s.MerchantEditWindowHours);
}