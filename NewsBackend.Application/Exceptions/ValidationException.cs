namespace NewsBackend.Application.Exceptions;

public sealed class ValidationException : ApiException
{
    public ValidationException(string message) : base(message, 400)
    {
    }
}