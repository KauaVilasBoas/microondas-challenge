using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingResumedEvent(Guid SessionId, int RemainingSeconds) : INotification;