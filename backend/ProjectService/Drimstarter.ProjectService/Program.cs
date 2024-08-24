using Drimstarter.Common.Grpc.Server;
using Drimstarter.Common.Validation.Behaviors;
using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Features.Categories;
using Drimstarter.ServiceDefaults;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // TODO: move to MediatorExtensions
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.AddNpgsqlDbContext<ProjectDbContext>(ResourceNames.ProjectServiceDb);

builder.Services.AddGrpcServer();

var app = builder.Build();

app.MapGrpcService<CategoryApi>();

app.Run();

public partial class Program;
