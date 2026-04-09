using Microondas.SharedKernel;

namespace Microondas.Domain.Heating.ValueObjects;

public sealed class HeatingTime : ValueObject
{
    public const int ManualMinimumSeconds = 1;
    public const int ManualMaximumSeconds = 120;

    public int TotalSeconds { get; }

    private HeatingTime(int totalSeconds) => TotalSeconds = totalSeconds;

    public static Result<HeatingTime> CreateManual(int seconds)
    {
        if (seconds < ManualMinimumSeconds)
            return Error.Validation("HeatingTime.BelowMinimum",
                $"Heating time must be at least {ManualMinimumSeconds} second.");

        if (seconds > ManualMaximumSeconds)
            return Error.Validation("HeatingTime.AboveMaximum",
                $"Heating time must not exceed {ManualMaximumSeconds} seconds (2 minutes).");

        return new HeatingTime(seconds);
    }

    public static Result<HeatingTime> CreateForProgram(int seconds)
    {
        if (seconds < ManualMinimumSeconds)
            return Error.Validation("HeatingTime.BelowMinimum",
                $"Heating time must be at least {ManualMinimumSeconds} second.");

        return new HeatingTime(seconds);
    }

    public static HeatingTime QuickStart => new(30);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalSeconds;
    }

    public override string ToString() => TotalSeconds.ToString();
}