using MediatR;
using Microondas.Application.Commands.Heating.PauseOrCancelHeating;
using Microondas.Application.Commands.Heating.QuickStartHeating;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Application.Commands.Heating.StartProgramHeating;
using Microondas.Application.ReadModels.Heating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

[ApiController]
[Route("api/heating")]
[Authorize]
public sealed class HeatingApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public HeatingApiController(IMediator mediator) => _mediator = mediator;

    [HttpGet("status")]
    public async Task<IActionResult> Status(CancellationToken ct)
    {
        var status = await _mediator.Send(new GetHeatingStatusQuery(), ct);
        return Ok(status ?? (object)new { status = "Idle" });
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartHeatingRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new StartHeatingCommand(request.TimeInSeconds, request.PowerLevel, request.HeatingChar), ct);

        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error.Description });
    }

    [HttpPost("quick-start")]
    public async Task<IActionResult> QuickStart(CancellationToken ct)
    {
        var result = await _mediator.Send(new QuickStartHeatingCommand(), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error.Description });
    }

    [HttpPost("pause-cancel")]
    public async Task<IActionResult> PauseOrCancel(CancellationToken ct)
    {
        var result = await _mediator.Send(new PauseOrCancelHeatingCommand(), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error.Description });
    }

    [HttpPost("start-program/{programId:guid}")]
    public async Task<IActionResult> StartProgram(Guid programId, CancellationToken ct)
    {
        var result = await _mediator.Send(new StartProgramHeatingCommand(programId), ct);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error.Description });
    }
}

public sealed record StartHeatingRequest(int? TimeInSeconds, int? PowerLevel, char? HeatingChar);