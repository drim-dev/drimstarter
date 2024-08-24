using Drimstarter.Common.Errors.Exceptions.Base;

namespace Drimstarter.Common.Errors.Exceptions;

public class InternalErrorException(string message, Exception? innerException = null)
    : ErrorException(message, innerException);
