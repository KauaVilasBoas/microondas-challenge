using MediatR;

namespace Microondas.Domain.Heating.Events;

public sealed record HeatingCancelledEvent(Guid SessionId) : INotification;
