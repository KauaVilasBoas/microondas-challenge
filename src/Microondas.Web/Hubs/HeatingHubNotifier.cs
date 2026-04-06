using Microsoft.AspNetCore.SignalR;
using Microondas.Application.EventHandlers;

namespace Microondas.Web.Hubs;

public sealed class HeatingHubNotifier : IHeatingHubNotifier
{
    private readonly IHubContext<HeatingHub> _hubContext;

    public HeatingHubNotifier(IHubContext<HeatingHub> hubContext) =>
        _hubContext = hubContext;

    public Task NotifyHeatingStartedAsync(Guid sessionId, int totalSeconds, int powerLevel, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingStarted", new { sessionId, totalSeconds, powerLevel }, cancellationToken);

    public Task NotifyHeatingTickedAsync(Guid sessionId, int remainingSeconds, string displayTime, string currentOutput, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingTicked", new { sessionId, remainingSeconds, displayTime, currentOutput }, cancellationToken);

    public Task NotifyHeatingPausedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingPaused", new { sessionId, remainingSeconds }, cancellationToken);

    public Task NotifyHeatingResumedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingResumed", new { sessionId, remainingSeconds }, cancellationToken);

    public Task NotifyHeatingCancelledAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingCancelled", new { sessionId }, cancellationToken);

    public Task NotifyHeatingCompletedAsync(Guid sessionId, string finalOutput, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingCompleted", new { sessionId, finalOutput }, cancellationToken);

    public Task NotifyHeatingTimeAddedAsync(Guid sessionId, int addedSeconds, int newRemainingSeconds, CancellationToken cancellationToken = default) =>
        _hubContext.Clients.All.SendAsync("HeatingTimeAdded", new { sessionId, addedSeconds, newRemainingSeconds }, cancellationToken);
}
