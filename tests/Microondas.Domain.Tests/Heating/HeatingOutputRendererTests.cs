using FluentAssertions;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class HeatingOutputRendererTests
{
    private readonly HeatingOutputRenderer _renderer = new();

    [Fact]
    public void RenderSegment_PowerOne_ReturnsSingleChar()
    {
        var character = HeatingCharacter.Create('.').Value;
        var power = PowerLevel.Create(1).Value;

        var result = _renderer.RenderSegment(character, power);

        result.Should().Be(".");
    }

    [Fact]
    public void RenderSegment_PowerThree_ReturnsThreeChars()
    {
        var character = HeatingCharacter.Create('.').Value;
        var power = PowerLevel.Create(3).Value;

        var result = _renderer.RenderSegment(character, power);

        result.Should().Be("...");
    }

    [Fact]
    public void RenderSegment_PowerTen_ReturnsTenChars()
    {
        var character = HeatingCharacter.Create('*').Value;
        var power = PowerLevel.Create(10).Value;

        var result = _renderer.RenderSegment(character, power);

        result.Should().Be("**********");
    }

    [Fact]
    public void RenderCompletion_AppendsCompletionMessage()
    {
        var result = _renderer.RenderCompletion("... ... ...");

        result.Should().Be("... ... ... Aquecimento concluído");
    }

    [Fact]
    public void RenderCompletion_WithEmptyOutput_ReturnsOnlyMessage()
    {
        var result = _renderer.RenderCompletion(string.Empty);

        result.Should().Be("Aquecimento concluído");
    }
}
