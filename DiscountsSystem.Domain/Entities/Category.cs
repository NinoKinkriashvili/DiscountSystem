namespace DiscountsSystem.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // normalized for uniqueness (trim + upper)
    public string NormalizedName { get; set; } = string.Empty;
    
    // Soft Delete
    public bool IsActive { get; set; } = true;
}