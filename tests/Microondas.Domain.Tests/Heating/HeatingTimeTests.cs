using FluentAssertions;
using Microondas.Domain.Heating.ValueObjects;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class HeatingTimeTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void CreateManual_WithValidSeconds_ReturnsSuccess(int seconds)
    {
        var result = HeatingTime.CreateManual(seconds);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalSeconds.Should().Be(seconds);
    }

    [Fact]
    public void CreateManual_WithZeroSeconds_ReturnsFailure()
    {
        var result = HeatingTime.CreateManual(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingTime.BelowMinimum");
    }

    [Fact]
    public void CreateManual_WithNegativeSeconds_ReturnsFailure()
    {
        var result = HeatingTime.CreateManual(-5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingTime.BelowMinimum");
    }

    [Theory]
    [InlineData(121)]
    [InlineData(200)]
    public void CreateManual_WithSecondsAbove120_ReturnsFailure(int seconds)
    {
        var result = HeatingTime.CreateManual(seconds);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingTime.AboveMaximum");
    }

    [Fact]
    public void QuickStart_Returns30Seconds()
    {
        HeatingTime.QuickStart.TotalSeconds.Should().Be(30);
    }

    [Fact]
    public void CreateForProgram_WithLargeValue_ReturnsSuccess()
    {
        var result = HeatingTime.CreateForProgram(840);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalSeconds.Should().Be(840);
    }
}
