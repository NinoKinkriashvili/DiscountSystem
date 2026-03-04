using DiscountsSystem.Domain.Entities;

namespace DiscountsSystem.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Category?> GetActiveByIdAsync(int id, CancellationToken ct = default);
    Task<List<Category>> GetActiveAsync(CancellationToken ct = default);

    Task AddAsync(Category category, CancellationToken ct = default);
    Task UpdateAsync(Category category, CancellationToken ct = default);
    Task<bool> DeactivateAsync(int id, CancellationToken ct = default);
    
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByNormalizedNameAsync(string normalizedName, int? excludeId = null, CancellationToken ct = default);
}