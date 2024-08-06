using Drimstarter.ProjectService.Database;
using Drimstarter.ProjectService.Features.Categories;
using Drimstarter.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.AddNpgsqlDbContext<ProjectDbContext>(ResourceNames.ProjectServiceDb);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CategoryApi>();

app.Run();

public partial class Program;
