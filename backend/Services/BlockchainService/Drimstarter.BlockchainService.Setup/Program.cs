using Drimstarter.BlockchainService.Database;
using Drimstarter.BlockchainService.Setup;
using Drimstarter.ServiceDefaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(SetupWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<BlockchainDbContext>(ResourceNames.BlockchainServiceDb);

builder.Services.AddHostedService<SetupWorker>();

var host = builder.Build();

host.Run();
