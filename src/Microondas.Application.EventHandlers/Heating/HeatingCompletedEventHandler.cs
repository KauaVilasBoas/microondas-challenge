using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingCompletedEventHandler : IDomainEventHandler<HeatingCompletedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingCompletedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingCompletedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingCompletedAsync(
            notification.SessionId,
            notification.FinalOutput,
            cancellationToken);
}
