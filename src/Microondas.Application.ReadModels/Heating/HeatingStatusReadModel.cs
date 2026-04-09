namespace Microondas.Application.ReadModels.Heating;

public sealed record HeatingStatusReadModel(
    string Status,
    int? RemainingSeconds,
    string? DisplayTime,
    int? ElapsedSeconds,
    string? CurrentOutput,
    int? PowerLevel,
    bool IsProgramSession,
    Guid? ProgramId);