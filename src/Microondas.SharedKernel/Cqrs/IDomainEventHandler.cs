using MediatR;

namespace Microondas.SharedKernel.Cqrs;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification
{
}
