using DiscountsSystem.Application.DTOs.Categories;
using DiscountsSystem.Application.Exceptions;
using DiscountsSystem.Application.Interfaces.Repositories;
using DiscountsSystem.Application.Interfaces.Services;
using DiscountsSystem.Application.Validation.Common;

namespace DiscountsSystem.Application.Services.Categories;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<List<CategoryListItemDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var categories = await _categories.GetActiveAsync(ct);

        return categories
            .Select(c => new CategoryListItemDto(c.Id, c.Name))
            .ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        return category is null
            ? null
            : new CategoryDto(category.Id, category.Name, category.IsActive);
    }

    public async Task<CategoryDto?> GetActiveByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _categories.GetActiveByIdAsync(id, ct);
        return category is null
            ? null
            : new CategoryDto(category.Id, category.Name, category.IsActive);
    }

    public async Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Category name is required.", nameof(request.Name));

        var name = request.Name.Trim();
        
        if (!NameRules.BeLatinNameWithSpaceOrHyphen(name))
            throw new ArgumentException(
                "Category name must contain only Latin letters and single spaces/hyphens.",
                nameof(request.Name));

        var normalized = name.ToUpperInvariant();

        var exists = await _categories.ExistsByNormalizedNameAsync(normalized, excludeId: null, ct);
        if (exists)
            throw new ConflictException($"Category with name '{name}' already exists.");

        var category = new DiscountsSystem.Domain.Entities.Category
        {
            Name = name,
            NormalizedName = normalized,
            IsActive = true
        };

        await _categories.AddAsync(category, ct);
        return category.Id;
    }

    public async Task<bool> UpdateNameAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Category name is required.", nameof(request.Name));

        var category = await _categories.GetByIdAsync(id, ct);
        if (category is null) 
            return false;

        var name = request.Name.Trim();
        if (!NameRules.BeLatinNameWithSpaceOrHyphen(name))
            throw new ArgumentException(
                "Category name must contain only Latin letters and single spaces/hyphens.",
                nameof(request.Name));
        
        var normalized = name.ToUpperInvariant();

        if (!string.Equals(category.NormalizedName, normalized, StringComparison.Ordinal))
        {
            var exists = await _categories.ExistsByNormalizedNameAsync(normalized, excludeId: id, ct);
            if (exists)
                throw new ConflictException($"Category with name '{name}' already exists.");
        }

        category.Name = name;
        category.NormalizedName = normalized;

        await _categories.UpdateAsync(category, ct);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateCategoryStatusRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _categories.GetByIdAsync(id, ct);
        if (category is null) return false;

        category.IsActive = request.IsActive;

        await _categories.UpdateAsync(category, ct);
        return true;
    }

    public Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
        => _categories.DeactivateAsync(id, ct);
}