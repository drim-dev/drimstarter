using Drimstarter.AccountService.Database;
using Drimstarter.AccountService.Features.Accounts;
using Drimstarter.Common.Database;
using Drimstarter.Common.Grpc.Server;
using Drimstarter.Common.Validation.Behaviors;
using Drimstarter.ServiceDefaults;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpcServer();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // TODO: move to MediatorExtensions
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.AddNpgsqlDbContext<AccountDbContext>(ResourceNames.AccountServiceDb);

// TODO: use different generator id for different replicas
builder.Services.AddIdFactory(1);

var app = builder.Build();

app.MapGrpcService<AccountApi>();

app.Run();

public partial class Program;
