using Drimstarter.ProjectService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Database;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapCategory(modelBuilder);
    }

    private static void MapCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(category =>
        {
            category.HasKey(c => c.Id);

            category.Property(c => c.Name)
                .HasMaxLength(Category.NameMaxLength)
                .IsRequired();

            category.HasIndex(c => c.Name)
                .IsUnique();
        });
    }
}
