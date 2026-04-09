using Microondas.Application.Commands.Behaviors;
using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Repositories;
using Microondas.Domain.Heating;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Programs;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Heating.StartProgramHeating;

public sealed class StartProgramHeatingCommandHandler : ICommandHandler<StartProgramHeatingCommand>
{
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly IHeatingOutputRenderer _renderer;
    private readonly IHeatingProgramRepository _programRepository;
    private readonly DomainEventCollector _collector;

    public StartProgramHeatingCommandHandler(
        HeatingSessionHolder sessionHolder,
        IHeatingOutputRenderer renderer,
        IHeatingProgramRepository programRepository,
        DomainEventCollector collector)
    {
        _sessionHolder = sessionHolder;
        _renderer = renderer;
        _programRepository = programRepository;
        _collector = collector;
    }

    public async Task<Result> Handle(StartProgramHeatingCommand command, CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync<HeatingProgram>(command.ProgramId, cancellationToken);

        if (program is null)
            return Error.NotFound("HeatingProgram.NotFound", $"Heating program '{command.ProgramId}' not found.");

        var parameters = HeatingParameters.Create(program.Time, program.Power, program.Character);
        var sessionResult = HeatingSession.Start(parameters, _renderer, program.Id);

        if (sessionResult.IsFailure) return Result.Failure(sessionResult.Error);

        _sessionHolder.Set(sessionResult.Value);
        _collector.Collect(sessionResult.Value);

        return Result.Success();
    }
}