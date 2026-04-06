using Microsoft.AspNetCore.Mvc;

namespace Microondas.Web.ViewComponents;

public sealed class DigitalKeyboardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string targetFieldId) =>
        View(targetFieldId);
}
