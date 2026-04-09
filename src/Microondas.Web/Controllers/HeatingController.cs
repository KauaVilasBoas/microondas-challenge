using MediatR;
using Microondas.Application.Commands.Heating.PauseOrCancelHeating;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Application.Commands.Heating.StartProgramHeating;
using Microondas.Application.ReadModels.Heating;
using Microondas.Web.Filters;
using Microondas.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Web.Controllers;

[TypeFilter(typeof(RequireApiAuthFilter))]
public sealed class HeatingController : Controller
{
    private readonly IMediator _mediator;

    public HeatingController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery());
        ViewData["HeatingStatus"] = status is null ? null : MapToStatusViewModel(status);

        return View(new HeatingViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(HeatingViewModel model)
    {
        var result = await _mediator.Send(
            new StartHeatingCommand(model.TimeInSeconds, model.PowerLevel, model.HeatingChar));

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

    /// <summary>
    /// JSON endpoint consumed by the SignalR fallback polling and status checks.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Status()
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery());
        if (status is null) return Json(new { status = "Idle" });
        return Json(MapToStatusViewModel(status));
    }

    private static HeatingStatusViewModel MapToStatusViewModel(HeatingStatusReadModel s) =>
        new()
        {
            Status = s.Status,
            RemainingSeconds = s.RemainingSeconds,
            DisplayTime = s.DisplayTime,
            ElapsedSeconds = s.ElapsedSeconds,
            CurrentOutput = s.CurrentOutput,
            PowerLevel = s.PowerLevel,
            IsProgramSession = s.IsProgramSession,
            ProgramId = s.ProgramId
        };
}