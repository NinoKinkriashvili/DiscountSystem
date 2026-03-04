using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscountsSystem.Infrastructure.Persistence.Context;

public class DiscountsDbContextFactory : IDesignTimeDbContextFactory<DiscountsDbContext>
{
    public DiscountsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DiscountsDbContext>();

        // connection string
        var cs = "Server=localhost,1433;Database=DiscountsSystemDb;User Id=sa;Password=Mimi1234.;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(cs);

        return new DiscountsDbContext(optionsBuilder.Options);
    }
}