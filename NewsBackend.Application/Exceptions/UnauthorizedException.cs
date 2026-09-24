namespace NewsBackend.Application.Exceptions;

public sealed class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) : base(message, 401)
    {
    }
}