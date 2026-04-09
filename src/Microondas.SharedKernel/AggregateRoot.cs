using MediatR;

namespace Microondas.SharedKernel;

public abstract class AggregateRoot : Entity
{
    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected void RaiseDomainEvent(INotification domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}