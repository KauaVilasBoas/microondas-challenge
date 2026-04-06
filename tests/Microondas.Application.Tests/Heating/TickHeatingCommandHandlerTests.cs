using FluentAssertions;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Application.Commands.Heating.TickHeating;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Xunit;

namespace Microondas.Application.Tests.Heating;

public sealed class TickHeatingCommandHandlerTests
{
    private readonly HeatingSessionHolder _holder = new();
    private readonly HeatingOutputRenderer _renderer = new();
    private readonly DomainEventCollector _collector = new();

    private async Task StartSession(int seconds = 5)
    {
        var startHandler = new StartHeatingCommandHandler(_holder, _renderer, _collector);
        await startHandler.Handle(new StartHeatingCommand(seconds, 10, null), CancellationToken.None);
    }

    private TickHeatingCommandHandler BuildHandler() =>
        new(_holder, _renderer, _collector);

    [Fact]
    public async Task Handle_WhenRunning_DecrementsRemainingSeconds()
    {
        await StartSession(10);
        var handler = BuildHandler();

        await handler.Handle(new TickHeatingCommand(), CancellationToken.None);

        _holder.Current!.RemainingSeconds.Should().Be(9);
    }

    [Fact]
    public async Task Handle_WhenSessionEnds_ClearsHolder()
    {
        await StartSession(1);
        var handler = BuildHandler();

        await handler.Handle(new TickHeatingCommand(), CancellationToken.None);

        _holder.Current.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenNoSession_ReturnsSuccess()
    {
        var handler = BuildHandler();

        var result = await handler.Handle(new TickHeatingCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_BuildsOutputString()
    {
        await StartSession(5);
        var handler = BuildHandler();

        await handler.Handle(new TickHeatingCommand(), CancellationToken.None);
        await handler.Handle(new TickHeatingCommand(), CancellationToken.None);

        _holder.Current!.CurrentOutput.Should().NotBeNullOrEmpty();
    }
}
