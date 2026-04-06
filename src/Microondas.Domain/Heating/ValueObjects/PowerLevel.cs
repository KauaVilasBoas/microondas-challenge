using Microondas.SharedKernel;

namespace Microondas.Domain.Heating.ValueObjects;

public sealed class PowerLevel : ValueObject
{
    public const int Minimum = 1;
    public const int Maximum = 10;
    public const int DefaultValue = 10;

    public int Value { get; }

    private PowerLevel(int value) => Value = value;

    public static Result<PowerLevel> Create(int value)
    {
        if (value < Minimum || value > Maximum)
            return Error.Validation("PowerLevel.OutOfRange",
                $"Power level must be between {Minimum} and {Maximum}.");

        return new PowerLevel(value);
    }

    public static PowerLevel Default => new(DefaultValue);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
