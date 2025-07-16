using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = exception.Message,
            ErrorType = exception.GetType().Name
        };

        var json = JsonSerializer.Serialize(response);

        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}
