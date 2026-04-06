using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingTimeAddedEventHandler : IDomainEventHandler<HeatingTimeAddedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingTimeAddedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingTimeAddedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingTimeAddedAsync(
            notification.SessionId,
            notification.AddedSeconds,
            notification.NewRemainingSeconds,
            cancellationToken);
}
