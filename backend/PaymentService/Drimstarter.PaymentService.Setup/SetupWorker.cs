namespace Drimstarter.PaymentService.Setup;

public class SetupWorker : BackgroundService
{
    private readonly ILogger<SetupWorker> _logger;

    public SetupWorker(ILogger<SetupWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
