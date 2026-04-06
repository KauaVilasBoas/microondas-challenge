using Microondas.SharedKernel;

namespace Microondas.Domain.Heating.ValueObjects;

public sealed class HeatingDisplayTime : ValueObject
{
    private readonly int _totalSeconds;

    private HeatingDisplayTime(int totalSeconds) => _totalSeconds = totalSeconds;

    public static HeatingDisplayTime From(int totalSeconds) => new(totalSeconds);

    public string Formatted => _totalSeconds is > 60 and < 100
        ? $"{_totalSeconds / 60}:{_totalSeconds % 60:D2}"
        : _totalSeconds.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _totalSeconds;
    }

    public override string ToString() => Formatted;
}
