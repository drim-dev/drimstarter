using System.Diagnostics;
using Drimstarter.Common.Database;
using Drimstarter.ProjectService.Database;

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

        _hostApplicationLifetime.StopApplication();
    }

    private async Task MigrateDatabase(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Client);

        await DatabaseMigrator.Migrate<ProjectDbContext>(_serviceProvider, stoppingToken);
    }
}
