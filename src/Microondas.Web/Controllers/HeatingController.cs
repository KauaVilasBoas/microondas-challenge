using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microondas.Application.Commands.Heating.PauseOrCancelHeating;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Application.Commands.Heating.StartProgramHeating;
using Microondas.Application.ReadModels.Heating;
using Microondas.Application.ReadModels.Programs;
using Microondas.Web.ViewModels;

namespace Microondas.Web.Controllers;

public sealed class HeatingController : Controller
{
    private readonly IMediator _mediator;

    public HeatingController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery());
        var programs = await _mediator.Send(new GetAllProgramsQuery());

        var statusVm = status is null ? null : MapToStatusViewModel(status);
        ViewData["HeatingStatus"] = statusVm;
        ViewData["Programs"] = programs;

        return View(new HeatingViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(HeatingViewModel model)
    {
        var command = new StartHeatingCommand(model.TimeInSeconds, model.PowerLevel, model.HeatingChar);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            model.ErrorMessage = result.Error.Description;
            return View("Index", model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartProgram(HeatingViewModel model)
    {
        if (!Guid.TryParse(model.SelectedProgramId, out var programId))
        {
            model.ErrorMessage = "Programa inválido selecionado.";
            return View("Index", model);
        }

        var result = await _mediator.Send(new StartProgramHeatingCommand(programId));

        if (result.IsFailure)
        {
            model.ErrorMessage = result.Error.Description;
            return View("Index", model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PauseOrCancel()
    {
        await _mediator.Send(new PauseOrCancelHeatingCommand());
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Status()
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery());
        if (status is null) return Json(new { status = "Idle" });
        return Json(MapToStatusViewModel(status));
    }

    private static HeatingStatusViewModel MapToStatusViewModel(HeatingStatusReadModel status) =>
        new()
        {
            Status = status.Status,
            RemainingSeconds = status.RemainingSeconds,
            DisplayTime = status.DisplayTime,
            ElapsedSeconds = status.ElapsedSeconds,
            CurrentOutput = status.CurrentOutput,
            PowerLevel = status.PowerLevel,
            IsProgramSession = status.IsProgramSession,
            ProgramId = status.ProgramId
        };
}
