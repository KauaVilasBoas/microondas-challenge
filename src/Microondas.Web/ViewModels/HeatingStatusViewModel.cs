namespace Microondas.Web.ViewModels;

public sealed class HeatingStatusViewModel
{
    public string Status { get; init; } = "Idle";
    public int? RemainingSeconds { get; init; }
    public string? DisplayTime { get; init; }
    public int? ElapsedSeconds { get; init; }
    public string? CurrentOutput { get; init; }
    public int? PowerLevel { get; init; }
    public bool IsProgramSession { get; init; }
    public Guid? ProgramId { get; init; }

    public bool IsRunning => Status == "Running";
    public bool IsPaused => Status == "Paused";
    public bool IsIdle => Status is "Idle" or "Completed" or "Cancelled";
}