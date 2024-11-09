using Drimstarter.BlockchainService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.BlockchainService.Database;

public class BlockchainDbContext : DbContext
{
    private const int BitcoinAddressMaxLength = 35;

    public BlockchainDbContext(DbContextOptions<BlockchainDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAddress(modelBuilder);
    }

    private static void MapAddress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(address =>
        {
            address.HasKey(a => a.Id);

            address.Property(a => a.BitcoinAddress)
                .HasMaxLength(BitcoinAddressMaxLength)
                .IsRequired();

            address.Property(a => a.UserId)
                .IsRequired();

            address.HasIndex(a => a.UserId)
                .IsUnique();

        });
    }
}
