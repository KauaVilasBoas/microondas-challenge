using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingStartedEvent(
    Guid SessionId,
    int TotalSeconds,
    int PowerLevel,
    char HeatingChar,
    Guid? ProgramId) : INotification;
