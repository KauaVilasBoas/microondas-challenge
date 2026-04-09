using System.Net;
using System.Text.Json;
using Microondas.Infrastructure.Logging;
using Microondas.SharedKernel;

namespace Microondas.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly FileExceptionLogger _exceptionLogger;

    public ApiExceptionMiddleware(RequestDelegate next, FileExceptionLogger exceptionLogger)
    {
        _next = next;
        _exceptionLogger = exceptionLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            await _exceptionLogger.LogAsync(ex, context.Request.Path);
            await WriteErrorResponse(context, HttpStatusCode.UnprocessableEntity, ex.ErrorCode, ex.Message);
        }
        catch (Exception ex)
        {
            await _exceptionLogger.LogAsync(ex, context.Request.Path);
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "InternalError",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string code,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { code, message });
        await context.Response.WriteAsync(payload);
    }
}