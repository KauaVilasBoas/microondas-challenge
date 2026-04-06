using Microondas.SharedKernel;

namespace Microondas.Domain.Heating.ValueObjects;

public sealed class HeatingParameters : ValueObject
{
    public HeatingTime Time { get; }
    public PowerLevel Power { get; }
    public HeatingCharacter Character { get; }

    private HeatingParameters(HeatingTime time, PowerLevel power, HeatingCharacter character)
    {
        Time = time;
        Power = power;
        Character = character;
    }

    public static HeatingParameters Create(HeatingTime time, PowerLevel power, HeatingCharacter character) =>
        new(time, power, character);

    public static HeatingParameters Default =>
        new(HeatingTime.QuickStart, PowerLevel.Default, HeatingCharacter.Default);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Time;
        yield return Power;
        yield return Character;
    }
}
