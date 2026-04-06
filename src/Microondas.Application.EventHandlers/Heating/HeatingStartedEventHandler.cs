using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingStartedEventHandler : IDomainEventHandler<HeatingStartedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingStartedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingStartedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingStartedAsync(
            notification.SessionId,
            notification.TotalSeconds,
            notification.PowerLevel,
            cancellationToken);
}
