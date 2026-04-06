using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Programs.Events;
using Microondas.Domain.Programs.ValueObjects;
using Microondas.SharedKernel;

namespace Microondas.Domain.Programs;

public sealed class HeatingProgram : AggregateRoot
{
    public ProgramName Name { get; private set; } = null!;
    public FoodDescription Food { get; private set; } = null!;
    public HeatingTime Time { get; private set; } = null!;
    public PowerLevel Power { get; private set; } = null!;
    public HeatingCharacter Character { get; private set; } = null!;
    public InstructionText Instructions { get; private set; } = null!;
    public ProgramType Type { get; private set; }

    public bool IsPredefined => Type == ProgramType.Predefined;
    public bool IsCustom => Type == ProgramType.Custom;

    private HeatingProgram() { }

    public static HeatingProgram CreatePredefined(
        string name,
        string food,
        int timeSeconds,
        int power,
        char character,
        string? instructions = null)
    {
        var program = new HeatingProgram
        {
            Name = ProgramName.Create(name).Value,
            Food = FoodDescription.Create(food).Value,
            Time = HeatingTime.CreateForProgram(timeSeconds).Value,
            Power = PowerLevel.Create(power).Value,
            Character = HeatingCharacter.Create(character).Value,
            Instructions = InstructionText.Create(instructions).Value,
            Type = ProgramType.Predefined
        };

        return program;
    }

    public static Result<HeatingProgram> CreateCustom(
        string name,
        string food,
        int timeSeconds,
        int power,
        char character,
        string? instructions = null)
    {
        var nameResult = ProgramName.Create(name);
        if (nameResult.IsFailure) return nameResult.Error;

        var foodResult = FoodDescription.Create(food);
        if (foodResult.IsFailure) return foodResult.Error;

        var timeResult = HeatingTime.CreateManual(timeSeconds);
        if (timeResult.IsFailure) return timeResult.Error;

        var powerResult = PowerLevel.Create(power);
        if (powerResult.IsFailure) return powerResult.Error;

        var charResult = HeatingCharacter.Create(character);
        if (charResult.IsFailure) return charResult.Error;

        var instructionsResult = InstructionText.Create(instructions);
        if (instructionsResult.IsFailure) return instructionsResult.Error;

        var program = new HeatingProgram
        {
            Name = nameResult.Value,
            Food = foodResult.Value,
            Time = timeResult.Value,
            Power = powerResult.Value,
            Character = charResult.Value,
            Instructions = instructionsResult.Value,
            Type = ProgramType.Custom
        };

        program.RaiseDomainEvent(new CustomProgramCreatedEvent(program.Id, name));

        return program;
    }

    public Result Delete()
    {
        if (IsPredefined)
            return Error.Forbidden("HeatingProgram.CannotDeletePredefined",
                "Pre-defined programs cannot be deleted.");

        RaiseDomainEvent(new CustomProgramDeletedEvent(Id, Name.Value));
        return Result.Success();
    }
}
