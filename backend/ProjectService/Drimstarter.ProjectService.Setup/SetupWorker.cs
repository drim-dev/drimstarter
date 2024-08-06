using System.Diagnostics;
using Drimstarter.Common.Database;
using Drimstarter.ProjectService.Database;

namespace Drimstarter.ProjectService.Setup;

public class SetupWorker(
    IServiceProvider _serviceProvider,
    IHostApplicationLifetime _hostApplicationLifetime)
    : BackgroundService
{
    public const string ActivitySourceName = "project-service-setup";

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrateDatabase(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }

    private async Task MigrateDatabase(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        await DatabaseMigrator.Migrate<ProjectDbContext>(_serviceProvider, stoppingToken);
    }
}
