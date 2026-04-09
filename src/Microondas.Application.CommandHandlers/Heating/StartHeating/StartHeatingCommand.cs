using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.Commands.Heating.StartHeating;

public sealed record StartHeatingCommand(
    int? TimeInSeconds,
    int? PowerLevel,
    char? HeatingChar) : ICommand;