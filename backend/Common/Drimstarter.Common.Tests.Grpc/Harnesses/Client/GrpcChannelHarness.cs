using Drimstarter.Common.Grpc.Client;
using Drimstarter.Common.Tests.Harnesses;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Drimstarter.Common.Tests.Grpc.Harnesses.Client;

public class GrpcChannelHarness<T> : IHarness<T>
    where T : class
{
    private HttpClient? _httpClient;
    private GrpcChannel? _grpcChannel;

    public GrpcChannel GrpcChannel => _grpcChannel ?? throw new InvalidOperationException("GrpcChannel is not initialized.");

    public void ConfigureWebHostBuilder(IWebHostBuilder builder)
    {
    }

    public Task Start(WebApplicationFactory<T> factory, CancellationToken cancellationToken)
    {
        _httpClient = factory.Server.CreateClient();

        _grpcChannel = GrpcChannel.ForAddress(_httpClient.BaseAddress!, new GrpcChannelOptions
        {
            LoggerFactory = new LoggerFactory(),
            HttpClient = _httpClient,
        });

        _grpcChannel.Intercept(new ClientExceptionInterceptor());

        return Task.CompletedTask;
    }

    public Task Stop(CancellationToken cancellationToken)
    {
        _grpcChannel?.Dispose();
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }
}
