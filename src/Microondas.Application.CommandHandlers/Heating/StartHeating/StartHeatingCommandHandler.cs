using Microondas.Application.Commands.Behaviors;
using Microondas.Domain.Contracts.Commands;
using Microondas.Domain.Heating;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Heating.StartHeating;

public sealed class StartHeatingCommandHandler : ICommandHandler<StartHeatingCommand>
{
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly IHeatingOutputRenderer _renderer;
    private readonly DomainEventCollector _collector;

    public StartHeatingCommandHandler(
        HeatingSessionHolder sessionHolder,
        IHeatingOutputRenderer renderer,
        DomainEventCollector collector)
    {
        _sessionHolder = sessionHolder;
        _renderer = renderer;
        _collector = collector;
    }

    public Task<Result> Handle(StartHeatingCommand command, CancellationToken cancellationToken)
    {
        var current = _sessionHolder.Current;

        if (current is { Status: HeatingStatus.Running })
        {
            var addResult = current.AddThirtySeconds();
            if (addResult.IsFailure) return Task.FromResult(addResult);

            _collector.Collect(current);
            return Task.FromResult(Result.Success());
        }

        if (current is { Status: HeatingStatus.Paused })
        {
            var resumeResult = current.Resume();
            if (resumeResult.IsFailure) return Task.FromResult(resumeResult);

            _collector.Collect(current);
            return Task.FromResult(Result.Success());
        }

        var timeResult = command.TimeInSeconds.HasValue
            ? HeatingTime.CreateManual(command.TimeInSeconds.Value)
            : Result<HeatingTime>.Success(HeatingTime.QuickStart);

        if (timeResult.IsFailure) return Task.FromResult(Result.Failure(timeResult.Error));

        var powerResult = command.PowerLevel.HasValue
            ? PowerLevel.Create(command.PowerLevel.Value)
            : Result<PowerLevel>.Success(PowerLevel.Default);

        if (powerResult.IsFailure) return Task.FromResult(Result.Failure(powerResult.Error));

        var charResult = command.HeatingChar.HasValue
            ? HeatingCharacter.Create(command.HeatingChar.Value)
            : Result<HeatingCharacter>.Success(HeatingCharacter.Default);

        if (charResult.IsFailure) return Task.FromResult(Result.Failure(charResult.Error));

        var parameters = HeatingParameters.Create(timeResult.Value, powerResult.Value, charResult.Value);
        var sessionResult = HeatingSession.Start(parameters, _renderer);

        if (sessionResult.IsFailure) return Task.FromResult(Result.Failure(sessionResult.Error));

        _sessionHolder.Set(sessionResult.Value);
        _collector.Collect(sessionResult.Value);

        return Task.FromResult(Result.Success());
    }
}