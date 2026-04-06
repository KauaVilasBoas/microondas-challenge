using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingCompletedEvent(Guid SessionId, string FinalOutput) : INotification;
