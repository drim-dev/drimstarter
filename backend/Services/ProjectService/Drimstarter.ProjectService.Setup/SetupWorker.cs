using System.Diagnostics;
using Drimstarter.Common.Database;
using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Domain;

namespace Drimstarter.ProjectService.Setup;

public class SetupWorker : BackgroundService
{
    public const string ActivitySourceName = "project-service-setup";

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public SetupWorker(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrateDatabase(stoppingToken);

        await SeedDatabase(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }

    private async Task MigrateDatabase(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        await DatabaseMigrator.Migrate<ProjectDbContext>(_serviceProvider, stoppingToken);
    }

    private async Task SeedDatabase(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        await using var scope = _serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
        var idFactory = scope.ServiceProvider.GetRequiredService<IdFactory>();

        await SeedCategoriesAndProjects(db, idFactory, stoppingToken);
    }

    private async Task SeedCategoriesAndProjects(ProjectDbContext db, IdFactory idFactory,
        CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        var categories = new List<Category>
        {
            new() { Id = idFactory.Create(), Name = "Category 1" },
            new() { Id = idFactory.Create(), Name = "Category 2" },
            new() { Id = idFactory.Create(), Name = "Category 3" }
        };

        await db.Categories.AddRangeAsync(categories, stoppingToken);
        await db.SaveChangesAsync(stoppingToken);

        var projects = new List<Project>
        {
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 1",
                Description = "Description 1",
                Story = "Story 1",
                FundingGoal = 100_000,
                CurrentFunding = 0,
                StartDate = new DateTime(2024, 12, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 12, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[0].Id,
                UserId = 1,
            },
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 2",
                Description = "Description 2",
                Story = "Story 2",
                FundingGoal = 30_000,
                CurrentFunding = 15_000,
                StartDate = new DateTime(2025, 7, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 10, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[1].Id,
                UserId = 1,
            },
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 3",
                Description = "Description 3",
                Story = "Story 3",
                FundingGoal = 50_000,
                CurrentFunding = 0,
                StartDate = new DateTime(2024, 3, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 3, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[2].Id,
                UserId = 1,
            },
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 4",
                Description = "Description 4",
                Story = "Story 4",
                FundingGoal = 400_000,
                CurrentFunding = 0,
                StartDate = new DateTime(2024, 12, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 12, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[0].Id,
                UserId = 1,
            },
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 5",
                Description = "Description 5",
                Story = "Story 5",
                FundingGoal = 100_000,
                CurrentFunding = 150_000,
                StartDate = new DateTime(2024, 8, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 9, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[1].Id,
                UserId = 1,
            },
            new()
            {
                Id = idFactory.Create(),
                Title = "Project 6",
                Description = "Description 6",
                Story = "Story 6",
                FundingGoal = 600_000,
                CurrentFunding = 0,
                StartDate = new DateTime(2024, 12, 01).ToUniversalTime(),
                EndDate = new DateTime(2025, 12, 01).ToUniversalTime(),
                Status = ProjectStatus.Draft,
                CategoryId = categories[2].Id,
                UserId = 1,
            }
        };

        await db.Projects.AddRangeAsync(projects, stoppingToken);
        await db.SaveChangesAsync(stoppingToken);
    }
}
