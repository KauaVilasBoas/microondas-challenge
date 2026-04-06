using Microondas.Domain.Heating.Events;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;
using Microondas.SharedKernel;

namespace Microondas.Domain.Heating;

public sealed class HeatingSession : AggregateRoot
{
    private const int ExtraSecondsPerPress = 30;

    public HeatingStatus Status { get; private set; }
    public HeatingParameters Parameters { get; private set; } = null!;
    public int RemainingSeconds { get; private set; }
    public int ElapsedSeconds { get; private set; }
    public string CurrentOutput { get; private set; } = string.Empty;
    public Guid? ProgramId { get; private set; }
    public bool IsProgramSession => ProgramId.HasValue;

    private HeatingSession() { }

    public static Result<HeatingSession> Start(
        HeatingParameters parameters,
        IHeatingOutputRenderer renderer,
        Guid? programId = null)
    {
        var session = new HeatingSession
        {
            Status = HeatingStatus.Running,
            Parameters = parameters,
            RemainingSeconds = parameters.Time.TotalSeconds,
            ElapsedSeconds = 0,
            CurrentOutput = string.Empty,
            ProgramId = programId
        };

        session.RaiseDomainEvent(new HeatingStartedEvent(
            session.Id,
            parameters.Time.TotalSeconds,
            parameters.Power.Value,
            parameters.Character.Value,
            programId));

        return session;
    }

    public static HeatingSession QuickStart(IHeatingOutputRenderer renderer)
    {
        var parameters = HeatingParameters.Default;
        var result = Start(parameters, renderer);
        return result.Value;
    }

    public Result Tick(IHeatingOutputRenderer renderer)
    {
        if (Status != HeatingStatus.Running)
            return Error.Validation("HeatingSession.NotRunning", "Cannot tick a session that is not running.");

        var segment = renderer.RenderSegment(Parameters.Character, Parameters.Power);
        CurrentOutput = ElapsedSeconds == 0
            ? segment
            : $"{CurrentOutput} {segment}";

        ElapsedSeconds++;
        RemainingSeconds--;

        if (RemainingSeconds <= 0)
        {
            return Complete(renderer);
        }

        var displayTime = HeatingDisplayTime.From(RemainingSeconds);
        RaiseDomainEvent(new HeatingTickedEvent(Id, RemainingSeconds, displayTime.Formatted, CurrentOutput));
        return Result.Success();
    }

    public Result Pause()
    {
        if (Status != HeatingStatus.Running)
            return Error.Validation("HeatingSession.NotRunning", "Cannot pause a session that is not running.");

        Status = HeatingStatus.Paused;
        RaiseDomainEvent(new HeatingPausedEvent(Id, RemainingSeconds));
        return Result.Success();
    }

    public Result Resume()
    {
        if (Status != HeatingStatus.Paused)
            return Error.Validation("HeatingSession.NotPaused", "Cannot resume a session that is not paused.");

        Status = HeatingStatus.Running;
        RaiseDomainEvent(new HeatingResumedEvent(Id, RemainingSeconds));
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != HeatingStatus.Paused)
            return Error.Validation("HeatingSession.NotPaused", "Cannot cancel a session that is not paused.");

        Status = HeatingStatus.Cancelled;
        RaiseDomainEvent(new HeatingCancelledEvent(Id));
        return Result.Success();
    }

    public Result AddThirtySeconds()
    {
        if (Status != HeatingStatus.Running)
            return Error.Validation("HeatingSession.NotRunning", "Can only add time to a running session.");

        if (IsProgramSession)
            return Error.Validation("HeatingSession.ProgramSession",
                "Cannot add extra time to a pre-defined program session.");

        RemainingSeconds += ExtraSecondsPerPress;
        RaiseDomainEvent(new HeatingTimeAddedEvent(Id, ExtraSecondsPerPress, RemainingSeconds));
        return Result.Success();
    }

    private Result Complete(IHeatingOutputRenderer renderer)
    {
        Status = HeatingStatus.Completed;
        CurrentOutput = renderer.RenderCompletion(CurrentOutput);
        RaiseDomainEvent(new HeatingCompletedEvent(Id, CurrentOutput));
        return Result.Success();
    }
}
