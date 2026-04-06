using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microondas.Application.ReadModels.Programs;

namespace Microondas.Web.ViewComponents;

public sealed class ProgramSelectorViewComponent : ViewComponent
{
    private readonly IMediator _mediator;

    public ProgramSelectorViewComponent(IMediator mediator) => _mediator = mediator;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var programs = await _mediator.Send(new GetAllProgramsQuery());
        return View(programs);
    }
}
