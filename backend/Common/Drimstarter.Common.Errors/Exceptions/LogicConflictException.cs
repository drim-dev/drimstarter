using Drimstarter.Common.Errors.Exceptions.Base;

namespace Drimstarter.Common.Errors.Exceptions;

public class LogicConflictException(string message, string code) : ErrorException(message)
{
    public string Code { get; } = code;
}
