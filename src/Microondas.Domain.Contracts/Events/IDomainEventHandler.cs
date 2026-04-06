using MediatR;

namespace Microondas.Domain.Contracts.Events;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification
{
}
