using Drimstarter.AccountService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.AccountService.Database;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapAccount(modelBuilder);
    }

    private static void MapAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(account =>
        {
            account.HasKey(a => a.Id);

            account.Property(a => a.Name)
                .HasMaxLength(Account.NameMaxLength)
                .IsRequired();

            account.Property(a => a.Email)
                .HasMaxLength(Account.EmailMaxLength)
                .IsRequired();

            account.Property(a => a.PasswordHash)
                .HasMaxLength(100)
                .IsRequired();

            account.HasIndex(a => a.Email)
                .IsUnique();
        });
    }
}
