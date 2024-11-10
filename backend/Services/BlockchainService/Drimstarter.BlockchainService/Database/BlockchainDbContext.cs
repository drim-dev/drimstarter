using Drimstarter.BlockchainService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.BlockchainService.Database;

public class BlockchainDbContext : DbContext
{
    /// <summary>
    /// The maximum length of a Bitcoin address.
    /// [Segwit] addresses are always between 14 and 74 characters long.
    /// Version 0 witness addresses are always 42 or 62 characters.
    /// Source: https://github.com/bitcoin/bips/blob/master/bip-0173.mediawiki
    /// </summary>
    private const int BitcoinAddressMaxLength = 74;

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

            address.HasIndex(a => a.UserId);

        });
    }
}
