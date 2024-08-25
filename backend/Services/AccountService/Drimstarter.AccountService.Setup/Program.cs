using Drimstarter.AccountService.Database;
using Drimstarter.AccountService.Setup;
using Drimstarter.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(SetupWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<AccountDbContext>(ResourceNames.AccountServiceDb);

builder.Services.AddHostedService<SetupWorker>();

var host = builder.Build();

host.Run();
