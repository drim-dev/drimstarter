using Drimstarter.BlockchainService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.BlockchainService.Database;

public class BlockchainDbContext : DbContext
{
    public BlockchainDbContext(DbContextOptions<BlockchainDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapCategory(modelBuilder);
    }

    private static void MapCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(category =>
        {
            category.HasKey(c => c.AddressId);
        });
    }
}
