using MediatR;

namespace Microondas.Domain.Programs.Events;

public sealed record CustomProgramCreatedEvent(Guid ProgramId, string ProgramName) : INotification;
