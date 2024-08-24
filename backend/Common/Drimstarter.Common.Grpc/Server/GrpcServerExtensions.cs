using Microsoft.Extensions.DependencyInjection;

namespace Drimstarter.Common.Grpc.Server;

public static class GrpcServerExtensions
{
    public static IServiceCollection AddGrpcServer(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ServerExceptionInterceptor>();
        });

        return services;
    }
}
