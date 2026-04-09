using Microondas.Application.Commands.Behaviors;
using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Heating;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Heating.QuickStartHeating;

public sealed class QuickStartHeatingCommandHandler : ICommandHandler<QuickStartHeatingCommand>
{
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly IHeatingOutputRenderer _renderer;
    private readonly DomainEventCollector _collector;

    public QuickStartHeatingCommandHandler(
        HeatingSessionHolder sessionHolder,
        IHeatingOutputRenderer renderer,
        DomainEventCollector collector)
    {
        _sessionHolder = sessionHolder;
        _renderer = renderer;
        _collector = collector;
    }

    public Task<Result> Handle(QuickStartHeatingCommand command, CancellationToken cancellationToken)
    {
        var current = _sessionHolder.Current;

        if (current is { Status: HeatingStatus.Running })
        {
            var addResult = current.AddThirtySeconds();
            if (addResult.IsFailure) return Task.FromResult(addResult);
            _collector.Collect(current);
            return Task.FromResult(Result.Success());
        }

        var parameters = HeatingParameters.Default;
        var sessionResult = HeatingSession.Start(parameters, _renderer);

        _sessionHolder.Set(sessionResult.Value);
        _collector.Collect(sessionResult.Value);

        return Task.FromResult(Result.Success());
    }
}