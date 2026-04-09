using Microondas.SharedKernel;

namespace Microondas.Domain.Programs.ValueObjects;

public sealed class ProgramName : ValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private ProgramName(string value) => Value = value;

    public static Result<ProgramName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("ProgramName.Empty", "Program name cannot be empty.");

        if (value.Length > MaxLength)
            return Error.Validation("ProgramName.TooLong",
                $"Program name cannot exceed {MaxLength} characters.");

        return new ProgramName(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}