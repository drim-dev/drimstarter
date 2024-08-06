using Drimstarter.Common.Web.Endpoints;
using Drimstarter.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddGrpcClient<Drimstarter.ProjectService.Client.Categories.CategoriesClient>(options =>
{
    options.Address = new Uri($"http://{ResourceNames.ProjectService}");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();

public partial class Program;
