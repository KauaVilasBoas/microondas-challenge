using FluentAssertions;
using Microondas.Domain.Heating.ValueObjects;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class PowerLevelTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_WithValidValue_ReturnsSuccess(int value)
    {
        var result = PowerLevel.Create(value);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void Create_WithInvalidValue_ReturnsFailure(int value)
    {
        var result = PowerLevel.Create(value);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PowerLevel.OutOfRange");
    }

    [Fact]
    public void Default_ReturnsPowerLevelOfTen()
    {
        PowerLevel.Default.Value.Should().Be(10);
    }

    [Fact]
    public void PowerLevelsWithSameValue_AreEqual()
    {
        var a = PowerLevel.Create(5).Value;
        var b = PowerLevel.Create(5).Value;

        a.Should().Be(b);
    }
}
