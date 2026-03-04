using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Repositories;

public sealed class SettingsRepository : ISettingsRepository
{
    private readonly DiscountsDbContext _db;

    public SettingsRepository(DiscountsDbContext db)
    {
        _db = db;
    }

    public Task<Settings?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Settings.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Settings?> GetCurrentAsync(CancellationToken ct = default)
        => _db.Settings.AsNoTracking().FirstOrDefaultAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}