using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Workshop.Domain.Exceptions;

namespace Workshop.API.Exceptions;

/// <summary>
/// Global exception handler that intercepts all unhandled exceptions and returns
/// standardized RFC 7807 ProblemDetails responses.
/// Registered via IExceptionHandler (.NET 8+).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        _logger.LogError(
            exception,
            "Unhandled exception [{ExceptionType}] caught by GlobalExceptionHandler: {Message}",
            exception.GetType().Name,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Returning true signals that the exception has been handled.
        return true;
    }

    private static (int statusCode, string title) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            BusinessRuleException => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
}
