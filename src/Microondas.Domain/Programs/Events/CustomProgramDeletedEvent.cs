using MediatR;

namespace Microondas.Domain.Programs.Events;

public sealed record CustomProgramDeletedEvent(Guid ProgramId, string ProgramName) : INotification;
