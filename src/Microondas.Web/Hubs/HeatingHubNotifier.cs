using Microsoft.AspNetCore.SignalR;
using Microondas.Application.EventHandlers;

namespace Microondas.Web.Hubs;

/// <summary>
/// Implements <see cref="IHeatingHubNotifier"/> by broadcasting events to all
/// connected SignalR clients via <see cref="HeatingHub"/>.
/// </summary>
public sealed class HeatingHubNotifier : IHeatingHubNotifier
{
    private readonly IHubContext<HeatingHub> _hub;

    public HeatingHubNotifier(IHubContext<HeatingHub> hub) => _hub = hub;

    public Task NotifyHeatingStartedAsync(
        Guid sessionId, int totalSeconds, int powerLevel, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingStarted",
            new { sessionId, totalSeconds, powerLevel },
            ct);

    public Task NotifyHeatingTickedAsync(
        Guid sessionId, int remainingSeconds, string displayTime, string currentOutput, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingTicked",
            new { sessionId, remainingSeconds, displayTime, currentOutput },
            ct);

    public Task NotifyHeatingPausedAsync(
        Guid sessionId, int remainingSeconds, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingPaused",
            new { sessionId, remainingSeconds },
            ct);

    public Task NotifyHeatingResumedAsync(
        Guid sessionId, int remainingSeconds, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingResumed",
            new { sessionId, remainingSeconds },
            ct);

    public Task NotifyHeatingCancelledAsync(
        Guid sessionId, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingCancelled",
            new { sessionId },
            ct);

    public Task NotifyHeatingCompletedAsync(
        Guid sessionId, string finalOutput, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingCompleted",
            new { sessionId, finalOutput },
            ct);

    public Task NotifyHeatingTimeAddedAsync(
        Guid sessionId, int addedSeconds, int newRemainingSeconds, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync(
            "HeatingTimeAdded",
            new { sessionId, addedSeconds, newRemainingSeconds },
            ct);
}
