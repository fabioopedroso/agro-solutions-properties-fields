using Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AgroSolutions.Properties.Fields.Middlewares;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro na requisição");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var message = "Ocorreu um erro interno no servidor.";

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status400BadRequest;
                message = validationException.Message;
                break;

            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                message = notFoundException.Message;
                break;

            case UnauthorizedException unauthorizedException:
                statusCode = StatusCodes.Status403Forbidden;
                message = unauthorizedException.Message;
                break;

            case BusinessException businessException:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                message = businessException.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            timestamp = DateTime.UtcNow
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
