using Microondas.Domain.Contracts.Commands;

namespace Microondas.Application.Commands.Programs.CreateCustomProgram;

public sealed record CreateCustomProgramCommand(
    string Name,
    string Food,
    int TimeInSeconds,
    int Power,
    char HeatingChar,
    string? Instructions) : ICommand;
