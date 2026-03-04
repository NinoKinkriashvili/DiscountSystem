using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DiscountsDbContext _db;

    public CategoryRepository(DiscountsDbContext db)
    {
        _db = db;
    }

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Category?> GetActiveByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, ct);

    public Task<List<Category>> GetActiveAsync(CancellationToken ct = default)
        => _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Category category, CancellationToken ct = default)
    {
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(Category category, CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null) 
            return false;

        category.IsActive = false;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    
    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        var normalized = name.Trim();

        var q = _db.Categories.AsNoTracking().Where(c => c.Name == normalized);

        if (excludeId.HasValue)
            q = q.Where(c => c.Id != excludeId.Value);

        return q.AnyAsync(ct);
    }

    public Task<bool> ExistsByNormalizedNameAsync(string normalizedName, int? excludeId = null, CancellationToken ct = default)
    {
        return _db.Categories.AsNoTracking()
            .AnyAsync(c =>
                    c.NormalizedName == normalizedName &&
                    (excludeId == null || c.Id != excludeId.Value),
                ct);
    }
}