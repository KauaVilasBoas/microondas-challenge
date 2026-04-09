using Microsoft.AspNetCore.Mvc;
using Microondas.Web.Services;
using Microondas.Web.ViewModels;

namespace Microondas.Web.Controllers;

public sealed class AuthController : Controller
{
    private readonly IApiAuthService _authService;
    private readonly ITokenStore _tokenStore;

    public AuthController(IApiAuthService authService, ITokenStore tokenStore)
    {
        _authService = authService;
        _tokenStore = tokenStore;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_tokenStore.IsAuthenticated())
            return RedirectToAction("Index", "Heating");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model.Username, model.Password);

        if (result.IsFailure)
        {
            model.ErrorMessage = result.Error.Description;
            return View(model);
        }

        _tokenStore.SetToken(result.Value);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Heating");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        _tokenStore.ClearToken();
        return RedirectToAction(nameof(Login));
    }
}
