using DiscountsSystem.Domain.Entities;
using DiscountsSystem.Infrastructure.Persistence.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscountsSystem.Infrastructure.Persistence.Context;

public class DiscountsDbContext : IdentityDbContext<AppUser>
{
    public DiscountsDbContext(DbContextOptions<DiscountsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<CouponPurchase> CouponPurchases => Set<CouponPurchase>();
    public DbSet<Settings> Settings => Set<Settings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountsDbContext).Assembly);
    }
}
