using FluentAssertions;
using Microondas.Domain.Heating.ValueObjects;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class HeatingDisplayTimeTests
{
    [Theory]
    [InlineData(90, "1:30")]
    [InlineData(61, "1:01")]
    [InlineData(99, "1:39")]
    [InlineData(70, "1:10")]
    [InlineData(75, "1:15")]
    public void Format_BetweenSixtyAndHundred_ReturnsMinutesAndSeconds(int seconds, string expected)
    {
        var displayTime = HeatingDisplayTime.From(seconds);

        displayTime.Formatted.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(30, "30")]
    [InlineData(60, "60")]
    [InlineData(100, "100")]
    [InlineData(120, "120")]
    public void Format_OutsideConversionRange_ReturnsRawSeconds(int seconds, string expected)
    {
        var displayTime = HeatingDisplayTime.From(seconds);

        displayTime.Formatted.Should().Be(expected);
    }

    [Fact]
    public void Format_Zero_ReturnsZero()
    {
        HeatingDisplayTime.From(0).Formatted.Should().Be("0");
    }
}
