using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingPausedEventHandler : IDomainEventHandler<HeatingPausedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingPausedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingPausedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingPausedAsync(
            notification.SessionId,
            notification.RemainingSeconds,
            cancellationToken);
}