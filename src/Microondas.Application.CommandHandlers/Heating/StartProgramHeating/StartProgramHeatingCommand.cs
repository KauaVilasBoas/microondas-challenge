using Microondas.Domain.Contracts.Commands;

namespace Microondas.Application.Commands.Heating.StartProgramHeating;

public sealed record StartProgramHeatingCommand(Guid ProgramId) : ICommand;