using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Setup;
using Drimstarter.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(SetupWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<ProjectDbContext>(ResourceNames.ProjectServiceDb);

builder.Services.AddHostedService<SetupWorker>();

var host = builder.Build();

host.Run();
