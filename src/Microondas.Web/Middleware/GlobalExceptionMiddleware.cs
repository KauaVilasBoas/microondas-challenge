using Microondas.Infrastructure.Logging;
using Microondas.SharedKernel;

namespace Microondas.Web.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly FileExceptionLogger _exceptionLogger;

    public GlobalExceptionMiddleware(RequestDelegate next, FileExceptionLogger exceptionLogger)
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
            context.Response.Redirect($"/Error?message={Uri.EscapeDataString(ex.Message)}");
        }
        catch (Exception ex)
        {
            await _exceptionLogger.LogAsync(ex, context.Request.Path);
            context.Response.Redirect("/Error");
        }
    }
}