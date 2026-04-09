using Microsoft.AspNetCore.Mvc;

namespace Microondas.Web.ViewComponents;

public sealed class DigitalKeyboardViewComponent : ViewComponent
{
    // View<TModel> garante que a string seja tratada como modelo, não como nome de view.
    public IViewComponentResult Invoke(string targetFieldId) =>
        View<string>("Default", targetFieldId);
}
