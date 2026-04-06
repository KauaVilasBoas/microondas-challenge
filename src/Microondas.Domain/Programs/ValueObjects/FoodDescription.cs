using Microondas.SharedKernel;

namespace Microondas.Domain.Programs.ValueObjects;

public sealed class FoodDescription : ValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private FoodDescription(string value) => Value = value;

    public static Result<FoodDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("FoodDescription.Empty", "Food description cannot be empty.");

        if (value.Length > MaxLength)
            return Error.Validation("FoodDescription.TooLong",
                $"Food description cannot exceed {MaxLength} characters.");

        return new FoodDescription(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
