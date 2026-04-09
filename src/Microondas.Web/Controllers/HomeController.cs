using Microsoft.AspNetCore.Mvc;

namespace Microondas.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Heating");

    public IActionResult Error([FromQuery] string? message)
    {
        ViewData["ErrorMessage"] = message ?? "Ocorreu um erro inesperado.";
        return View();
    }
}