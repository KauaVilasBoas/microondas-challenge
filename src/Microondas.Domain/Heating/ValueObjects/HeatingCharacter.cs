using Microondas.SharedKernel;

namespace Microondas.Domain.Heating.ValueObjects;

public sealed class HeatingCharacter : ValueObject
{
    public const char DefaultChar = '.';

    public char Value { get; }

    private HeatingCharacter(char value) => Value = value;

    public static Result<HeatingCharacter> Create(char value)
    {
        if (char.IsWhiteSpace(value))
            return Error.Validation("HeatingCharacter.Whitespace",
                "Heating character cannot be a whitespace character.");

        return new HeatingCharacter(value);
    }

    public static HeatingCharacter Default => new(DefaultChar);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}