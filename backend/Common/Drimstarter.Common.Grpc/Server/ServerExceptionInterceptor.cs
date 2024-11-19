using Drimstarter.Common.Errors.Exceptions.Base;
using Drimstarter.Common.Grpc.Shared;
using Drimstarter.Common.Grpc.Shared.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Drimstarter.Common.Grpc.Server;

// TODO: check and test logic
public class ServerExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ErrorException ex)
        {
            throw RpcExceptionConverter.Convert(ex);
        }
        catch (Exception ex)
        {
            // Log unhandled exception
            throw RpcExceptionConverter.Convert(ex);
        }
    }
}
