using FluentAssertions;
using Microondas.Domain.Programs;
using Xunit;

namespace Microondas.Domain.Tests.Programs;

public sealed class PredefinedProgramSeedTests
{
    [Fact]
    public void GetAll_ReturnsFivePrograms()
    {
        var programs = PredefinedProgramSeed.GetAll();

        programs.Should().HaveCount(5);
    }

    [Fact]
    public void GetAll_AllProgramsArePredefined()
    {
        var programs = PredefinedProgramSeed.GetAll();

        programs.Should().AllSatisfy(p => p.IsPredefined.Should().BeTrue());
    }

    [Fact]
    public void GetAll_AllHaveUniqueCharacters()
    {
        var programs = PredefinedProgramSeed.GetAll();
        var chars = programs.Select(p => p.Character.Value).ToList();

        chars.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetAll_ContainsExpectedProgramNames()
    {
        var programs = PredefinedProgramSeed.GetAll();
        var names = programs.Select(p => p.Name.Value).ToList();

        names.Should().Contain("Pipoca");
        names.Should().Contain("Leite");
        names.Should().Contain("Carnes de boi");
        names.Should().Contain("Frango");
        names.Should().Contain("Feijão");
    }

    [Fact]
    public void ReservedCharacters_IncludesDefaultChar()
    {
        var reserved = PredefinedProgramSeed.ReservedCharacters;

        reserved.Should().Contain('.');
    }

    [Fact]
    public void ReservedCharacters_IncludesAllProgramChars()
    {
        var programs = PredefinedProgramSeed.GetAll();
        var reserved = PredefinedProgramSeed.ReservedCharacters;

        foreach (var program in programs)
        {
            reserved.Should().Contain(program.Character.Value);
        }
    }
}