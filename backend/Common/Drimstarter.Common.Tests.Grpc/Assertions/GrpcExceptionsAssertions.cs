using Drimstarter.Common.Errors.Exceptions;
using Drimstarter.Common.Grpc.Shared.Exceptions;
using FluentAssertions;
using Grpc.Core;

namespace Drimstarter.Common.Tests.Grpc.Assertions;

public static class GrpcExceptionsAssertions
{
    public static async Task ShouldThrowValidationErrorsException(this Func<Task> action, string field, string message,
        string code)
    {
        var rpcEx = (await action.Should().ThrowAsync<RpcException>()).Which;
        var ex = RpcExceptionConverter.Convert(rpcEx);
        var validationEx = ex as ValidationErrorsException;

        validationEx.Should().NotBeNull();
        validationEx!.Errors.Should().HaveCount(1);
        var error = validationEx.Errors.Single();
        error.Should().BeEquivalentTo(new RequestValidationError(field, message, code));
    }
}
