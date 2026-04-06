using Microondas.Application.EventHandlers;

namespace Microondas.Api.Notifications;

public sealed class NoOpHeatingHubNotifier : IHeatingHubNotifier
{
    public Task NotifyHeatingStartedAsync(Guid sessionId, int totalSeconds, int powerLevel, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingTickedAsync(Guid sessionId, int remainingSeconds, string displayTime, string currentOutput, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingPausedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingResumedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingCancelledAsync(Guid sessionId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingCompletedAsync(Guid sessionId, string finalOutput, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task NotifyHeatingTimeAddedAsync(Guid sessionId, int addedSeconds, int newRemainingSeconds, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
