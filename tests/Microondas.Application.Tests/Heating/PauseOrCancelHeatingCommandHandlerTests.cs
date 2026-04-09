using FluentAssertions;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.Commands.Heating.PauseOrCancelHeating;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Xunit;

namespace Microondas.Application.Tests.Heating;

public sealed class PauseOrCancelHeatingCommandHandlerTests
{
    private readonly HeatingSessionHolder _holder = new();
    private readonly HeatingOutputRenderer _renderer = new();
    private readonly DomainEventCollector _collector = new();

    private async Task StartSession()
    {
        var startHandler = new StartHeatingCommandHandler(_holder, _renderer, _collector);
        await startHandler.Handle(new StartHeatingCommand(30, 10, null), CancellationToken.None);
    }

    private PauseOrCancelHeatingCommandHandler BuildHandler() =>
        new(_holder, _collector);

    [Fact]
    public async Task Handle_WhenRunning_PausesSession()
    {
        await StartSession();
        var handler = BuildHandler();

        var result = await handler.Handle(new PauseOrCancelHeatingCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current!.Status.Should().Be(HeatingStatus.Paused);
    }

    [Fact]
    public async Task Handle_WhenPaused_CancelsSession()
    {
        await StartSession();
        _holder.Current!.Pause();
        var handler = BuildHandler();

        var result = await handler.Handle(new PauseOrCancelHeatingCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenIdle_ClearsHolder()
    {
        var handler = BuildHandler();

        var result = await handler.Handle(new PauseOrCancelHeatingCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current.Should().BeNull();
    }
}