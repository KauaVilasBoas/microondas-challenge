using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.Commands.Heating.PauseOrCancelHeating;

public sealed record PauseOrCancelHeatingCommand : ICommand;