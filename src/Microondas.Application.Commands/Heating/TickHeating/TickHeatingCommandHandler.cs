using Microondas.Application.Commands.Behaviors;
using Microondas.Domain.Contracts.Commands;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Heating.TickHeating;

public sealed class TickHeatingCommandHandler : ICommandHandler<TickHeatingCommand>
{
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly IHeatingOutputRenderer _renderer;
    private readonly DomainEventCollector _collector;

    public TickHeatingCommandHandler(
        HeatingSessionHolder sessionHolder,
        IHeatingOutputRenderer renderer,
        DomainEventCollector collector)
    {
        _sessionHolder = sessionHolder;
        _renderer = renderer;
        _collector = collector;
    }

    public Task<Result> Handle(TickHeatingCommand command, CancellationToken cancellationToken)
    {
        var current = _sessionHolder.Current;

        if (current is null || current.Status != HeatingStatus.Running)
            return Task.FromResult(Result.Success());

        var result = current.Tick(_renderer);

        if (current.Status is HeatingStatus.Completed or HeatingStatus.Cancelled)
            _sessionHolder.Clear();

        _collector.Collect(current);

        return Task.FromResult(result);
    }
}
