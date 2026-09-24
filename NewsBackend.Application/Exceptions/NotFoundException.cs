namespace NewsBackend.Application.Exceptions;

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}