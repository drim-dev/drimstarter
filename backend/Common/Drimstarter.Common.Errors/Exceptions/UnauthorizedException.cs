using Drimstarter.Common.Errors.Exceptions.Base;

namespace Drimstarter.Common.Errors.Exceptions;

public class UnauthorizedException() : ErrorException("Request is unauthorized");
