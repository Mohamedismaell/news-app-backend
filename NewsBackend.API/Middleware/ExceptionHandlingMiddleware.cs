using System.Text.Json;
using NewsBackend.Application.Exceptions;

namespace NewsBackend.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Request {Method} {Path} was cancelled by the client.",
                context.Request.Method,
                context.Request.Path);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(
                "Request {Method} {Path} failed with status code {StatusCode}: {Message}",
                context.Request.Method,
                context.Request.Path,
                ex.StatusCode,
                ex.Message);

            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogInformation(
                "Request {Method} {Path} contained a malformed request body: {Message}",
                context.Request.Method,
                context.Request.Path,
                ex.Message);

            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Invalid request body.");
        }
        catch (JsonException)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "Invalid request body.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing request {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(
            new ErrorResponse(statusCode, message, errors, context.TraceIdentifier));
    }
}