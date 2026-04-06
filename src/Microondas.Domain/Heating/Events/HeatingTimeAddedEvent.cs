using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingTimeAddedEvent(Guid SessionId, int AddedSeconds, int NewRemainingSeconds) : INotification;
