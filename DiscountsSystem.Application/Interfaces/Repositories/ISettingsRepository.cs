using DiscountsSystem.Domain.Entities;

namespace DiscountsSystem.Application.Interfaces.Repositories;

public interface ISettingsRepository
{
    Task<Settings?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Settings?> GetCurrentAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}