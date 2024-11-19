using Drimstarter.ProjectService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Drimstarter.ProjectService.Database;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapCategory(modelBuilder);
        MapProject(modelBuilder);
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

    private static void MapProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(project =>
        {
            project.HasKey(p => p.Id);

            project.Property(p => p.Title)
                .HasMaxLength(Project.TitleMaxLength)
                .IsRequired();

            project.Property(p => p.Description)
                .HasMaxLength(Project.DescriptionMaxLength)
                .IsRequired();

            project.Property(p => p.Story)
                .HasMaxLength(Project.StoryMaxLength)
                .IsRequired();

            project.Property(p => p.FundingGoal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            project.Property(p => p.CurrentFunding)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            project.Property(p => p.StartDate)
                .IsRequired();

            project.Property(p => p.EndDate)
                .IsRequired();

            project.Property(p => p.Status)
                .IsRequired();

            project.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            project.HasIndex(p => p.CategoryId);

            project.HasIndex(p => p.UserId);

            project.HasIndex(p => p.Status);
        });
    }
}
