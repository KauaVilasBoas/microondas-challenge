using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Heating.Events;

namespace Microondas.Application.EventHandlers.Heating;

public sealed class HeatingResumedEventHandler : IDomainEventHandler<HeatingResumedEvent>
{
    private readonly IHeatingHubNotifier _hubNotifier;

    public HeatingResumedEventHandler(IHeatingHubNotifier hubNotifier) =>
        _hubNotifier = hubNotifier;

    public async Task Handle(HeatingResumedEvent notification, CancellationToken cancellationToken) =>
        await _hubNotifier.NotifyHeatingResumedAsync(
            notification.SessionId,
            notification.RemainingSeconds,
            cancellationToken);
}