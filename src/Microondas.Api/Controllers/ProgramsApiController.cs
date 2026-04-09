using MediatR;
using Microondas.Application.Commands.Programs.CreateCustomProgram;
using Microondas.Application.Commands.Programs.DeleteCustomProgram;
using Microondas.Application.ReadModels.Programs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

[ApiController]
[Route("api/programs")]
[Authorize]
public sealed class ProgramsApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProgramsApiController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var programs = await _mediator.Send(new GetAllProgramsQuery(), ct);
        return Ok(programs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var program = await _mediator.Send(new GetProgramByIdQuery(id), ct);
        if (program is null) return NotFound(new { message = $"Program '{id}' not found." });
        return Ok(program);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomProgramRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CreateCustomProgramCommand(
                request.Name, request.Food, request.TimeInSeconds,
                request.Power, request.HeatingChar, request.Instructions), ct);

        return result.IsSuccess ? Created() : BadRequest(new { error = result.Error.Description });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCustomProgramCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error.Description });
    }
}

public sealed record CreateCustomProgramRequest(
    string Name,
    string Food,
    int TimeInSeconds,
    int Power,
    char HeatingChar,
    string? Instructions);