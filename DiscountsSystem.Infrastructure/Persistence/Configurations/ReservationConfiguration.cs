using DiscountsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountsSystem.Infrastructure.Persistence.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .HasMaxLength(450) // Identity-safe
            .IsRequired();

        builder.Property(x => x.OfferId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();

        builder.HasOne(x => x.Offer)
            .WithMany()
            .HasForeignKey(x => x.OfferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CustomerId, x.OfferId, x.Status });
        builder.HasIndex(x => new { x.Status, x.ExpiresAtUtc });

        builder.HasIndex(x => new { x.CustomerId, x.OfferId })
            .IsUnique()
            .HasFilter("[Status] = 0");
    }
}
