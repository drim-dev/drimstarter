using System.Net;
using Drimstarter.Common.Grpc.Server;
using Drimstarter.Common.Tests.Grpc.Harnesses.Service.Utils;
using Drimstarter.Common.Tests.Harnesses;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.Common.Tests.Grpc.Harnesses.Service.Base;

public abstract class BaseServiceHarness<TProgram> : IHarness<TProgram>
    where TProgram : class
{
    private readonly int _port;

    private readonly WebApplication _webApplication;
    private bool _started;

    private readonly string _serviceName;

    protected BaseServiceHarness(string serviceName)
    {
        _serviceName = serviceName;

        _port = NetUtils.GetAvailablePort();

        var webApplicationBuilder = WebApplication.CreateBuilder();

        webApplicationBuilder.Services.AddGrpcServer();
        webApplicationBuilder.Services.AddSingleton<FakeMediatorSender>();
        webApplicationBuilder.Services.AddSingleton<ISender>(provider => provider.GetRequiredService<FakeMediatorSender>());

        webApplicationBuilder.WebHost.UseUrls($"http://0.0.0.0:{_port}");
        webApplicationBuilder.WebHost.ConfigureKestrel((_, options) =>
        {
            options.Listen(IPAddress.Any, _port, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
        });

        _webApplication = webApplicationBuilder.Build();

        // ReSharper disable once VirtualMemberCallInConstructor
        MapGrpcServices(_webApplication);
    }

    protected abstract void MapGrpcServices(WebApplication webApplication);

    public void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable($"services__{_serviceName}__http__0", $"http://localhost:{_port}");
    }

    public async Task Start(WebApplicationFactory<TProgram> factory, CancellationToken cancellationToken)
    {
        await _webApplication.StartAsync(cancellationToken);

        _started = true;
    }

    public async Task Stop(CancellationToken cancellationToken)
    {
        await _webApplication.StopAsync(cancellationToken);

        _started = false;
    }

    public SingleResponseMethodMock<TRequest, TResponse> MockResponse<TRequest, TResponse>(Func<TRequest, TResponse> responseFactory)
        where TRequest : IRequest<TResponse>
    {
        ThrowIfNotStarted();

        return _webApplication.Services.GetRequiredService<FakeMediatorSender>().MockResponse(responseFactory);
    }

    public void Reset()
    {
        ThrowIfNotStarted();

        _webApplication.Services.GetRequiredService<FakeMediatorSender>().Reset();
    }

    private void ThrowIfNotStarted()
    {
        if (!_started)
        {
            throw new InvalidOperationException($"{_serviceName} harness is not started. Call {nameof(Start)} first.");
        }
    }
}
