using FluentAssertions;
using Microondas.Domain.Programs;
using Xunit;

namespace Microondas.Domain.Tests.Programs;

public sealed class HeatingProgramTests
{
    [Fact]
    public void CreatePredefined_SetsTypePredefined()
    {
        var program = HeatingProgram.CreatePredefined("Pipoca", "Pipoca", 180, 7, 'p');

        program.IsPredefined.Should().BeTrue();
        program.IsCustom.Should().BeFalse();
    }

    [Fact]
    public void CreateCustom_WithValidData_SetsTypeCustom()
    {
        var result = HeatingProgram.CreateCustom("Meu Programa", "Comida", 60, 5, 'x');

        result.IsSuccess.Should().BeTrue();
        result.Value.IsCustom.Should().BeTrue();
        result.Value.IsPredefined.Should().BeFalse();
    }

    [Fact]
    public void CreateCustom_WithInvalidPower_ReturnsFailure()
    {
        var result = HeatingProgram.CreateCustom("Prog", "Comida", 60, 11, 'x');

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CreateCustom_WithInvalidTime_ReturnsFailure()
    {
        var result = HeatingProgram.CreateCustom("Prog", "Comida", 0, 5, 'x');

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Delete_WhenPredefined_ReturnsFailure()
    {
        var program = HeatingProgram.CreatePredefined("Pipoca", "Pipoca", 180, 7, 'p');

        var result = program.Delete();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingProgram.CannotDeletePredefined");
    }

    [Fact]
    public void Delete_WhenCustom_Succeeds()
    {
        var program = HeatingProgram.CreateCustom("Meu Programa", "Comida", 60, 5, 'x').Value;

        var result = program.Delete();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateCustom_RaisesCustomProgramCreatedEvent()
    {
        var result = HeatingProgram.CreateCustom("Prog", "Comida", 60, 5, 'x');

        result.Value.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "CustomProgramCreatedEvent");
    }
}