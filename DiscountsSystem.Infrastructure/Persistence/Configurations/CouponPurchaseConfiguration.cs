using DiscountsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountsSystem.Infrastructure.Persistence.Configurations;

public sealed class CouponPurchaseConfiguration : IEntityTypeConfiguration<CouponPurchase>
{
    public void Configure(EntityTypeBuilder<CouponPurchase> builder)
    {
        builder.ToTable("CouponPurchases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.OfferId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.CouponCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SourceType)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.Offer)
            .WithMany()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => new { x.CustomerId, x.Status });

        builder.HasIndex(x => new { x.Status, x.ExpiresAtUtc });

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.CouponCode)
            .IsUnique();

        builder.HasIndex(x => x.ReservationId)
            .IsUnique()
            .HasFilter("[ReservationId] IS NOT NULL");
    }
}
