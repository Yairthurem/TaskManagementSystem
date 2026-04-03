using System.Net;
using System.Text.Json;

namespace TaskManagementSystem.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (KeyNotFoundException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { Message = ex.Message }));
        }
        catch (Exception ex)
        {
            // Log exact error and full stack trace deeply specifically per request
            _logger.LogError(ex, "FATAL UNHANDLED ERROR: {Message}. Full StackTrace: {StackTrace}", ex.Message, ex.StackTrace);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new 
            {
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected internal server error occurred.",
                DetailedError = ex.Message // Exposing strictly per dev-pattern
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
