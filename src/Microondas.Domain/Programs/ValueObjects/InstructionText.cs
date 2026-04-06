using Microondas.SharedKernel;

namespace Microondas.Domain.Programs.ValueObjects;

public sealed class InstructionText : ValueObject
{
    public const int MaxLength = 500;

    public string? Value { get; }

    private InstructionText(string? value) => Value = value;

    public static InstructionText Empty => new(null);

    public static Result<InstructionText> Create(string? value)
    {
        if (value is null) return new InstructionText(null);

        if (value.Length > MaxLength)
            return Error.Validation("InstructionText.TooLong",
                $"Instructions cannot exceed {MaxLength} characters.");

        return new InstructionText(value.Trim());
    }

    public bool HasContent => !string.IsNullOrWhiteSpace(Value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? string.Empty;
    }

    public override string ToString() => Value ?? string.Empty;
}
