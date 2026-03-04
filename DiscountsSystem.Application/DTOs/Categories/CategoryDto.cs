namespace DiscountsSystem.Application.DTOs.Categories;

public record CategoryDto(
    int Id,
    string Name,
    bool IsActive
);