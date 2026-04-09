using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingTickedEventHandler : IDomainEventHandler<HeatingTickedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingTickedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingTickedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingTickedAsync(
            notification.SessionId,
            notification.RemainingSeconds,
            notification.DisplayTime,
            notification.CurrentOutput,
            cancellationToken);
}