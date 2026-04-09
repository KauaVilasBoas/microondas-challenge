namespace Microondas.Application.ReadModels.Programs;

public sealed record ProgramReadModel(
    Guid Id,
    string Name,
    string Food,
    int TimeInSeconds,
    int Power,
    char HeatingChar,
    string Type,
    bool IsCustom,
    string? Instructions);