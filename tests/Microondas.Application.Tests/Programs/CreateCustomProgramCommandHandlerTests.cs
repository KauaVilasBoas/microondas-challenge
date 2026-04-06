using FluentAssertions;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.Commands.Programs.CreateCustomProgram;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Programs;
using Moq;
using Xunit;

namespace Microondas.Application.Tests.Programs;

public sealed class CreateCustomProgramCommandHandlerTests
{
    private readonly Mock<IHeatingProgramRepository> _repositoryMock = new();
    private readonly DomainEventCollector _collector = new();

    private CreateCustomProgramCommandHandler BuildHandler() =>
        new(_repositoryMock.Object, _collector);

    [Fact]
    public async Task Handle_WithUniqueChar_CreatesProgram()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCharacterAsync('x', It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateCustomProgramCommand("Meu Prog", "Comida", 60, 5, 'x', null);
        var handler = BuildHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HeatingProgram>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateChar_ReturnsConflictError()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCharacterAsync('x', It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateCustomProgramCommand("Meu Prog", "Comida", 60, 5, 'x', null);
        var handler = BuildHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingCharacter.Duplicate");
    }

    [Fact]
    public async Task Handle_WithReservedChar_ReturnsConflictError()
    {
        var command = new CreateCustomProgramCommand("Meu Prog", "Comida", 60, 5, '.', null);
        var handler = BuildHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingCharacter.Reserved");
    }

    [Fact]
    public async Task Handle_CollectsDomainEvents()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCharacterAsync('x', It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateCustomProgramCommand("Meu Prog", "Comida", 60, 5, 'x', null);
        var handler = BuildHandler();

        await handler.Handle(command, CancellationToken.None);

        _collector.DomainEvents.Should().NotBeEmpty();
    }
}
