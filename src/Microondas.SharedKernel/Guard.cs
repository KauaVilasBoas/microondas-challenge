namespace Microondas.SharedKernel;

public static class Guard
{
    public static T AgainstNull<T>(T? value, string parameterName) where T : class =>
        value ?? throw new ArgumentNullException(parameterName);

    public static string AgainstNullOrWhiteSpace(string? value, string parameterName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{parameterName} cannot be null or whitespace.", parameterName)
            : value;

    public static int AgainstOutOfRange(int value, string parameterName, int min, int max)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName,
                $"{parameterName} must be between {min} and {max}. Value was {value}.");
        return value;
    }

    public static T AgainstInvalidEnum<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException($"Invalid enum value '{value}' for {parameterName}.", parameterName);
        return value;
    }
}
