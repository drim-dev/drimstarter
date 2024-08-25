using Drimstarter.Common.Grpc.Client;
using Drimstarter.Common.Web.Endpoints;
using Drimstarter.Common.Web.Errors;
using Drimstarter.ServiceDefaults;
using Grpc.Net.ClientFactory;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// TODO: move to GrpcClientExtensions
builder.Services.AddScoped<ClientExceptionInterceptor>();

builder.Services.AddGrpcClient<Drimstarter.ProjectService.Client.Categories.CategoriesClient>(options =>
{
    options.Address = new Uri($"http://{ResourceNames.ProjectService}");
    options.InterceptorRegistrations.Add(new InterceptorRegistration(InterceptorScope.Channel, p => p.GetRequiredService<ClientExceptionInterceptor>()));
});

builder.Services.AddGrpcClient<Drimstarter.AccountService.Client.Accounts.AccountsClient>(options =>
{
    options.Address = new Uri($"http://{ResourceNames.AccountService}");
    options.InterceptorRegistrations.Add(new InterceptorRegistration(InterceptorScope.Channel, p => p.GetRequiredService<ClientExceptionInterceptor>()));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapProblemDetails();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();

public partial class Program;
