using Microondas.Application.Commands.Behaviors;
using Microondas.Domain.Contracts.Commands;
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
        var current = _sessionHolder.Current;

        if (current is null || current.Status == HeatingStatus.Idle)
        {
            _sessionHolder.Clear();
            return Task.FromResult(Result.Success());
        }

        if (current.Status == HeatingStatus.Running)
        {
            var pauseResult = current.Pause();
            if (pauseResult.IsFailure) return Task.FromResult(pauseResult);
            _collector.Collect(current);
            return Task.FromResult(Result.Success());
        }

        if (current.Status == HeatingStatus.Paused)
        {
            var cancelResult = current.Cancel();
            if (cancelResult.IsFailure) return Task.FromResult(cancelResult);
            _collector.Collect(current);
            _sessionHolder.Clear();
            return Task.FromResult(Result.Success());
        }

        _sessionHolder.Clear();
        return Task.FromResult(Result.Success());
    }
}
