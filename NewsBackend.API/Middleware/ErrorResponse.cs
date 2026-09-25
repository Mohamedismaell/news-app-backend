namespace NewsBackend.API.Middleware;

public class ErrorResponse
{
    public ErrorResponse(int statusCode, string message, IReadOnlyDictionary<string, string[]>? errors, string traceId)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
        TraceId = traceId;
    }

    public int StatusCode { get; }

    public string Message { get; }

    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public string TraceId { get; }
}