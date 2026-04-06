using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microondas.Application.ReadModels.Heating;
using Microondas.Web.ViewModels;

namespace Microondas.Web.ViewComponents;

public sealed class HeatingDisplayViewComponent : ViewComponent
{
    private readonly IMediator _mediator;

    public HeatingDisplayViewComponent(IMediator mediator) => _mediator = mediator;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery());

        if (status is null)
            return View(new HeatingStatusViewModel());

        return View(new HeatingStatusViewModel
        {
            Status = status.Status,
            RemainingSeconds = status.RemainingSeconds,
            DisplayTime = status.DisplayTime,
            ElapsedSeconds = status.ElapsedSeconds,
            CurrentOutput = status.CurrentOutput,
            PowerLevel = status.PowerLevel,
            IsProgramSession = status.IsProgramSession,
            ProgramId = status.ProgramId
        });
    }
}
