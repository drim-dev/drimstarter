using Drimstarter.PaymentService.Setup;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<SetupWorker>();

var host = builder.Build();

host.Run();
