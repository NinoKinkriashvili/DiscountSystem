using DiscountsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountsSystem.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NormalizedName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
        builder.HasIndex(x => x.NormalizedName)
            .IsUnique();
    }
}
