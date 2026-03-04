using DiscountsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountsSystem.Infrastructure.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(o => o.OriginalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.DiscountPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.CouponQuantityTotal)
            .IsRequired();

        builder.Property(o => o.CouponQuantityAvailable)
            .IsRequired();

        builder.Property(o => o.StartDateUtc)
            .IsRequired();

        builder.Property(o => o.EndDateUtc)
            .IsRequired();

        // Enum -> int (default)
        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(o => o.MerchantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.CreatedAtUtc)
            .IsRequired();

        builder.Property(o => o.UpdatedAtUtc)
            .IsRequired();

        builder.HasOne(o => o.Category)
            .WithMany()
            .HasForeignKey(o => o.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.CategoryId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.EndDateUtc);
        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken()
            .IsRequired();
    }
}
