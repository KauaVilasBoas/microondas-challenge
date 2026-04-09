using Microondas.Domain.Contracts.Commands;

namespace Microondas.Application.Commands.Programs.DeleteCustomProgram;

public sealed record DeleteCustomProgramCommand(Guid ProgramId) : ICommand;