using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.Commands.Programs.DeleteCustomProgram;

public sealed record DeleteCustomProgramCommand(Guid ProgramId) : ICommand;