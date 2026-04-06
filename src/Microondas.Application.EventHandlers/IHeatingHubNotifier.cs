namespace Microondas.Application.EventHandlers;

public interface IHeatingHubNotifier
{
    Task NotifyHeatingStartedAsync(Guid sessionId, int totalSeconds, int powerLevel, CancellationToken cancellationToken = default);
    Task NotifyHeatingTickedAsync(Guid sessionId, int remainingSeconds, string displayTime, string currentOutput, CancellationToken cancellationToken = default);
    Task NotifyHeatingPausedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default);
    Task NotifyHeatingResumedAsync(Guid sessionId, int remainingSeconds, CancellationToken cancellationToken = default);
    Task NotifyHeatingCancelledAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task NotifyHeatingCompletedAsync(Guid sessionId, string finalOutput, CancellationToken cancellationToken = default);
    Task NotifyHeatingTimeAddedAsync(Guid sessionId, int addedSeconds, int newRemainingSeconds, CancellationToken cancellationToken = default);
}
