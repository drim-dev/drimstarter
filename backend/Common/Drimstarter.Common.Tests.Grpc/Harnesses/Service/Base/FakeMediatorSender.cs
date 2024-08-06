using System.Linq.Expressions;
using MediatR;
using Moq;

namespace Drimstarter.Common.Tests.Grpc.Harnesses.Service.Base;

public class FakeMediatorSender : ISender
{
    private readonly Mock<ISender> _mock = new();

    public SingleResponseMethodMock<TRequest, TResponse> MockResponse<TRequest, TResponse>(
        Func<TRequest, TResponse> responseFactory)
        where TRequest : IRequest<TResponse>
    {
        var methodMock = new SingleResponseMethodMock<TRequest, TResponse>();

        _mock
            .Setup(x => x.Send(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TRequest request, CancellationToken _) =>
            {
                var response = responseFactory(request);
                methodMock.SetResult(request, response);
                return response;
            });

        return methodMock;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new())
    {
        var requestSetup = _mock.Setups.FirstOrDefault(x => (x.Expression.Body as MethodCallExpression)?
            .Arguments.First().Type.GenericTypeArguments[0] == typeof(TResponse));

        return requestSetup == null
            ? Task.FromException<TResponse>(new Exception("No configuration for request"))
            : _mock.Object.Send(request, cancellationToken);
    }

    public void Reset() => _mock.Reset();

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new())
        where TRequest : IRequest
    {
        throw new NotImplementedException();
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}

public class SingleResponseMethodMock<TRequest, TResponse>
{
    public void SetResult(TRequest request, TResponse response)
    {
        if (Invoked)
        {
            throw new Exception("Result already set");
        }

        Request = request;
        Response = response;
        Invoked = true;
    }

    public TRequest Request { get; private set; }
    public TResponse Response { get; private set; }
    public bool Invoked { get; private set; }
}
