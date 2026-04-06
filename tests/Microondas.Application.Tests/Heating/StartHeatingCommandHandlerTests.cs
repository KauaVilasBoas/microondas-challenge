using FluentAssertions;
using Microondas.Application.Commands.Behaviors;
using Microondas.Application.Commands.Heating.StartHeating;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Xunit;

namespace Microondas.Application.Tests.Heating;

public sealed class StartHeatingCommandHandlerTests
{
    private readonly HeatingSessionHolder _holder = new();
    private readonly HeatingOutputRenderer _renderer = new();
    private readonly DomainEventCollector _collector = new();

    private StartHeatingCommandHandler BuildHandler() =>
        new(_holder, _renderer, _collector);

    [Fact]
    public async Task Handle_WithValidTimeAndPower_StartsSession()
    {
        var command = new StartHeatingCommand(30, 5, null);
        var handler = BuildHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current.Should().NotBeNull();
        _holder.Current!.Status.Should().Be(HeatingStatus.Running);
        _holder.Current.RemainingSeconds.Should().Be(30);
    }

    [Fact]
    public async Task Handle_WithNoParams_UsesDefaults()
    {
        var command = new StartHeatingCommand(null, null, null);
        var handler = BuildHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current!.Parameters.Power.Value.Should().Be(10);
        _holder.Current.RemainingSeconds.Should().Be(30);
    }

    [Fact]
    public async Task Handle_WhenAlreadyRunning_AddsThirtySeconds()
    {
        var command = new StartHeatingCommand(30, 10, null);
        var handler = BuildHandler();

        await handler.Handle(command, CancellationToken.None);
        var originalRemaining = _holder.Current!.RemainingSeconds;

        await handler.Handle(command, CancellationToken.None);

        _holder.Current.RemainingSeconds.Should().Be(originalRemaining + 30);
    }

    [Fact]
    public async Task Handle_WhenPaused_ResumesSession()
    {
        var command = new StartHeatingCommand(30, 10, null);
        var handler = BuildHandler();

        await handler.Handle(command, CancellationToken.None);
        _holder.Current!.Pause();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _holder.Current.Status.Should().Be(HeatingStatus.Running);
    }

    [Fact]
    public async Task Handle_CollectsDomainEvents()
    {
        var command = new StartHeatingCommand(30, 10, null);
        var handler = BuildHandler();

        await handler.Handle(command, CancellationToken.None);

        _collector.DomainEvents.Should().NotBeEmpty();
    }
}
