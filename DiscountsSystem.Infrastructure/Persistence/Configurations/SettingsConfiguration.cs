using DiscountsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountsSystem.Infrastructure.Persistence.Configurations;

public sealed class SettingsConfiguration : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.ToTable("Settings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ReservationDurationMinutes)
            .IsRequired();

        builder.Property(x => x.MerchantEditWindowHours)
            .IsRequired();
    }
}
