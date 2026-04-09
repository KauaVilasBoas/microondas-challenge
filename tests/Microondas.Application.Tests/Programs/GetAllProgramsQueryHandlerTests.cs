using FluentAssertions;
using Microondas.Application.ReadModels.Programs;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Programs;
using Moq;
using Xunit;

namespace Microondas.Application.Tests.Programs;

public sealed class GetAllProgramsQueryHandlerTests
{
    private readonly Mock<IHeatingProgramRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_ReturnsAllPredefinedAndCustomPrograms()
    {
        var customProgram = HeatingProgram.CreateCustom("Custom", "Food", 60, 5, 'x').Value;

        _repositoryMock
            .Setup(r => r.GetAllAsync<HeatingProgram>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HeatingProgram> { customProgram }.AsReadOnly());

        var handler = new GetAllProgramsQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllProgramsQuery(), CancellationToken.None);

        result.Should().HaveCount(6);
        result.Count(p => !p.IsCustom).Should().Be(5);
        result.Count(p => p.IsCustom).Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithNoCustomPrograms_ReturnsFivePredefined()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync<HeatingProgram>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HeatingProgram>().AsReadOnly());

        var handler = new GetAllProgramsQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllProgramsQuery(), CancellationToken.None);

        result.Should().HaveCount(5);
        result.Should().AllSatisfy(p => p.IsCustom.Should().BeFalse());
    }

    [Fact]
    public async Task Handle_CustomPrograms_AreMarkedAsCustom()
    {
        var customProgram = HeatingProgram.CreateCustom("Custom", "Food", 60, 5, 'x').Value;

        _repositoryMock
            .Setup(r => r.GetAllAsync<HeatingProgram>(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HeatingProgram> { customProgram }.AsReadOnly());

        var handler = new GetAllProgramsQueryHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetAllProgramsQuery(), CancellationToken.None);

        result.Single(p => p.IsCustom).Name.Should().Be("Custom");
    }
}