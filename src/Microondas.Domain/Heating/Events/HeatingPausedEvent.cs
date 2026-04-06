using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingPausedEvent(Guid SessionId, int RemainingSeconds) : INotification;
