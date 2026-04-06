using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microondas.Application.Commands.Programs.CreateCustomProgram;
using Microondas.Application.Commands.Programs.DeleteCustomProgram;
using Microondas.Application.ReadModels.Programs;
using Microondas.Web.ViewModels;

namespace Microondas.Web.Controllers;

public sealed class ProgramsController : Controller
{
    private readonly IMediator _mediator;

    public ProgramsController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var programs = await _mediator.Send(new GetAllProgramsQuery());
        return View(new ProgramListViewModel { Programs = programs });
    }

    public IActionResult Create() => View(new CreateProgramViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProgramViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var command = new CreateCustomProgramCommand(
            model.Name,
            model.Food,
            model.TimeInSeconds,
            model.Power,
            model.HeatingChar,
            model.Instructions);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            model.ErrorMessage = result.Error.Description;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteCustomProgramCommand(id));

        if (result.IsFailure)
            TempData["ErrorMessage"] = result.Error.Description;

        return RedirectToAction(nameof(Index));
    }
}
