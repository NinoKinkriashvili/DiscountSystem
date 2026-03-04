using DiscountsSystem.Application.DTOs.Categories;

namespace DiscountsSystem.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryListItemDto>> GetActiveAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoryDto?> GetActiveByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);

    Task<bool> UpdateNameAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(int id, UpdateCategoryStatusRequest request, CancellationToken ct = default);

    Task<bool> DeactivateAsync(int id, CancellationToken ct = default); // optional (თუ გინდა DELETE-იც)
}