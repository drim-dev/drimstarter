using Drimstarter.Common.Grpc.Shared;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Drimstarter.Common.Grpc.Client;

// TODO: check and test logic
public class ClientExceptionInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        var response = HandleResponse(call.ResponseAsync, context.Options.CancellationToken);

        return new AsyncUnaryCall<TResponse>(response, call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
    }

    private static async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> responseTask,
        CancellationToken cancellationToken)
    {
        try
        {
            return await responseTask;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled && cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (RpcException ex)
        {
            var exception = RpcExceptionConverter.Convert(ex);

            throw exception;
        }
        catch (Exception)
        {
            // TODO: log unhandled exception
            throw;
        }
    }
}
