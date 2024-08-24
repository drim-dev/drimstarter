using System.Text.Json;
using Drimstarter.Common.Errors.Exceptions;
using Grpc.Core;

namespace Drimstarter.Common.Grpc.Shared;

// TODO: check and test logic
public static class RpcExceptionConverter
{
    private const string ValidationResultKey = "validation_result";
    private const string LogicConflictCodeKey = "logic_conflict_code";

    public static Exception Convert(RpcException rpcException)
    {
        switch (rpcException.StatusCode)
        {
            case StatusCode.InvalidArgument:
            {
                var validationResultData = rpcException.Trailers.GetValueBytes(ValidationResultKey);
                if (validationResultData == null)
                {
                    throw new InternalErrorException(
                        $"RpcException with statusCode {StatusCode.InvalidArgument} has unexpected content",
                        rpcException);
                }

                var errors = JsonSerializer.Deserialize<RequestValidationError[]>(validationResultData);
                return new ValidationErrorsException(errors!);
            }
            case StatusCode.FailedPrecondition:
            {
                var logicCode = rpcException.Trailers.GetValue(LogicConflictCodeKey);
                if (string.IsNullOrEmpty(logicCode))
                {
                    throw new InternalErrorException(
                        $"RpcException with statusCode {StatusCode.FailedPrecondition} has unexpected content",
                        rpcException);
                }

                return new LogicConflictException(logicCode, rpcException.Status.Detail);
            }
            case StatusCode.Internal:
                return new InternalErrorException(rpcException.Status.Detail);
            default:
                return new InternalErrorException("Generic RPC exception in gRPC client", rpcException);
        }
    }

    public static RpcException Convert(Exception exception)
    {
        switch (exception)
        {
            case ValidationErrorsException validationErrorsException:
            {
                var validationResultBinary = JsonSerializer.Serialize(validationErrorsException.Errors);
                return new RpcException(new Status(StatusCode.InvalidArgument, validationErrorsException.Message),
                    new Metadata { new(ValidationResultKey, validationResultBinary) });
            }
            case LogicConflictException logicConflictException:
            {
                var metadata = new Metadata { new(LogicConflictCodeKey, logicConflictException.Code) };

                return new RpcException(new Status(StatusCode.FailedPrecondition, logicConflictException.Message), metadata);
            }
            default:
            {
                return new RpcException(new Status(StatusCode.Internal, exception.Message));
            }
        }
    }
}
