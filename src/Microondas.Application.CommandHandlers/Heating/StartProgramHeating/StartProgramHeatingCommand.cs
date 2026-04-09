using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.Commands.Heating.StartProgramHeating;

public sealed record StartProgramHeatingCommand(Guid ProgramId) : ICommand;