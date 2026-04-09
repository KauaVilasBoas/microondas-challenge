using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingCancelledEventHandler : IDomainEventHandler<HeatingCancelledEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingCancelledEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingCancelledEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingCancelledAsync(notification.SessionId, cancellationToken);
}