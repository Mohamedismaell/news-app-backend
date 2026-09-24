namespace NewsBackend.Application.Exceptions;

public sealed class ForbiddenException : ApiException
{
    public ForbiddenException(string message) : base(message, 403)
    {
    }
}