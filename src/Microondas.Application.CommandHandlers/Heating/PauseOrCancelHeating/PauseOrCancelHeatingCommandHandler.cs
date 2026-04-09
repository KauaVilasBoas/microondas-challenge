using Microondas.Application.Commands.Behaviors;
using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Heating.PauseOrCancelHeating;

public sealed class PauseOrCancelHeatingCommandHandler : ICommandHandler<PauseOrCancelHeatingCommand>
{
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly DomainEventCollector _collector;

    public PauseOrCancelHeatingCommandHandler(
        HeatingSessionHolder sessionHolder,
        DomainEventCollector collector)
    {
        _sessionHolder = sessionHolder;
        _collector = collector;
    }

    public Task<Result> Handle(PauseOrCancelHeatingCommand command, CancellationToken cancellationToken)
    {
        var currentHeatingSession = _sessionHolder.Current;

        if (currentHeatingSession is null || currentHeatingSession.Status == HeatingStatus.Idle)
        {
            _sessionHolder.Clear();
            return Task.FromResult(Result.Success());
        }

        if (currentHeatingSession.Status == HeatingStatus.Running)
        {
            var pauseResult = currentHeatingSession.Pause();
            if (pauseResult.IsFailure) return Task.FromResult(pauseResult);
            _collector.Collect(currentHeatingSession);
            return Task.FromResult(Result.Success());
        }

        if (currentHeatingSession.Status == HeatingStatus.Paused)
        {
            var cancelResult = currentHeatingSession.Cancel();
            if (cancelResult.IsFailure) return Task.FromResult(cancelResult);
            _collector.Collect(currentHeatingSession);
            _sessionHolder.Clear();
            return Task.FromResult(Result.Success());
        }

        _sessionHolder.Clear();
        return Task.FromResult(Result.Success());
    }
}