using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingTickedEvent(
    Guid SessionId,
    int RemainingSeconds,
    string DisplayTime,
    string CurrentOutput) : INotification;