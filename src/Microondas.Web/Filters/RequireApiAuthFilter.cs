using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microondas.Web.Services;

namespace Microondas.Web.Filters;

public sealed class RequireApiAuthFilter : IAsyncActionFilter
{
    private readonly ITokenStore _tokenStore;

    public RequireApiAuthFilter(ITokenStore tokenStore)
        => _tokenStore = tokenStore;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_tokenStore.IsAuthenticated())
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            return;
        }

        await next();
    }
}
