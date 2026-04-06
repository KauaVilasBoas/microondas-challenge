using MediatR;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Behaviors;

public sealed class DomainEventCollector
{
    private readonly List<INotification> _events = [];

    public IReadOnlyList<INotification> DomainEvents => _events.AsReadOnly();

    public void Collect(AggregateRoot aggregate)
    {
        _events.AddRange(aggregate.DomainEvents);
        aggregate.ClearDomainEvents();
    }

    public void Clear() => _events.Clear();
}
