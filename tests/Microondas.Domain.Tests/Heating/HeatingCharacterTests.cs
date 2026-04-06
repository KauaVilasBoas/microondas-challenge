using FluentAssertions;
using Microondas.Domain.Heating.ValueObjects;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class HeatingCharacterTests
{
    [Theory]
    [InlineData('.')]
    [InlineData('*')]
    [InlineData('#')]
    [InlineData('a')]
    public void Create_WithValidChar_ReturnsSuccess(char c)
    {
        var result = HeatingCharacter.Create(c);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(c);
    }

    [Theory]
    [InlineData(' ')]
    [InlineData('\t')]
    [InlineData('\n')]
    public void Create_WithWhitespace_ReturnsFailure(char c)
    {
        var result = HeatingCharacter.Create(c);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingCharacter.Whitespace");
    }

    [Fact]
    public void Default_ReturnsDot()
    {
        HeatingCharacter.Default.Value.Should().Be('.');
    }
}
