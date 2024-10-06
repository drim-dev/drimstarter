using Drimstarter.BlockchainService.Database;
using Drimstarter.BlockchainService.Features.CryptoPayments;
using Drimstarter.Common.Database;
using Drimstarter.Common.Grpc.Server;
using Drimstarter.Common.Validation.Behaviors;
using Drimstarter.ServiceDefaults;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // TODO: move to MediatorExtensions
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.AddNpgsqlDbContext<BlockchainDbContext>(ResourceNames.BlockchainServiceDb);

builder.Services.AddGrpcServer();

// TODO: use different generator id for different replicas
builder.Services.AddIdFactory(1);

var app = builder.Build();

app.MapGrpcService<CryptoPaymentsApi>();

app.Run();

public partial class Program;
