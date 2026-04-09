using FluentAssertions;
using Microondas.Domain.Heating;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;
using Xunit;

namespace Microondas.Domain.Tests.Heating;

public sealed class HeatingSessionTests
{
    private static readonly HeatingOutputRenderer Renderer = new();

    private static HeatingParameters BuildParameters(int seconds = 30, int power = 10, char c = '.')
    {
        var time = HeatingTime.CreateManual(seconds).Value;
        var powerLevel = PowerLevel.Create(power).Value;
        var character = HeatingCharacter.Create(c).Value;
        return HeatingParameters.Create(time, powerLevel, character);
    }

    [Fact]
    public void Start_WithValidParameters_ReturnsRunningSession()
    {
        var parameters = BuildParameters();
        var result = HeatingSession.Start(parameters, Renderer);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(HeatingStatus.Running);
        result.Value.RemainingSeconds.Should().Be(30);
        result.Value.ElapsedSeconds.Should().Be(0);
    }

    [Fact]
    public void QuickStart_SetsThirtySecondsAndPowerTen()
    {
        var session = HeatingSession.QuickStart(Renderer);

        session.Status.Should().Be(HeatingStatus.Running);
        session.RemainingSeconds.Should().Be(30);
        session.Parameters.Power.Value.Should().Be(10);
    }

    [Fact]
    public void Tick_WhenRunning_DecrementsRemainingSecond()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 10), Renderer).Value;

        session.Tick(Renderer);

        session.RemainingSeconds.Should().Be(9);
        session.ElapsedSeconds.Should().Be(1);
    }

    [Fact]
    public void Tick_WhenRemainingReachesZero_CompletesSession()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 1), Renderer).Value;

        session.Tick(Renderer);

        session.Status.Should().Be(HeatingStatus.Completed);
        session.RemainingSeconds.Should().BeLessOrEqualTo(0);
    }

    [Fact]
    public void Tick_WhenCompleted_AppendsCompletionMessage()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 1), Renderer).Value;

        session.Tick(Renderer);

        session.CurrentOutput.Should().Contain("Aquecimento concluído");
    }

    [Fact]
    public void Tick_BuildsCorrectOutputFormat()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 5, power: 3, c: '.'), Renderer).Value;

        session.Tick(Renderer);
        session.Tick(Renderer);

        session.CurrentOutput.Should().Be("... ...");
    }

    [Fact]
    public void Pause_WhenRunning_TransitionsToPaused()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;

        var result = session.Pause();

        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(HeatingStatus.Paused);
    }

    [Fact]
    public void Pause_WhenNotRunning_ReturnsFailure()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;
        session.Pause();

        var result = session.Pause();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Resume_WhenPaused_TransitionsToRunning()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;
        session.Pause();

        var result = session.Resume();

        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(HeatingStatus.Running);
    }

    [Fact]
    public void Cancel_WhenPaused_TransitionsToCancelled()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;
        session.Pause();

        var result = session.Cancel();

        result.IsSuccess.Should().BeTrue();
        session.Status.Should().Be(HeatingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenRunning_ReturnsFailure()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;

        var result = session.Cancel();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddThirtySeconds_WhenRunning_IncreasesRemaining()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 30), Renderer).Value;

        var result = session.AddThirtySeconds();

        result.IsSuccess.Should().BeTrue();
        session.RemainingSeconds.Should().Be(60);
    }

    [Fact]
    public void AddThirtySeconds_WhenProgramSession_ReturnsFailure()
    {
        var timeResult = HeatingTime.CreateForProgram(180).Value;
        var power = PowerLevel.Create(7).Value;
        var character = HeatingCharacter.Create('p').Value;
        var parameters = HeatingParameters.Create(timeResult, power, character);
        var session = HeatingSession.Start(parameters, Renderer, Guid.NewGuid()).Value;

        var result = session.AddThirtySeconds();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HeatingSession.ProgramSession");
    }

    [Fact]
    public void AddThirtySeconds_WhenNotRunning_ReturnsFailure()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;
        session.Pause();

        var result = session.AddThirtySeconds();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Start_RaisesHeatingStartedEvent()
    {
        var parameters = BuildParameters();
        var session = HeatingSession.Start(parameters, Renderer).Value;

        session.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "HeatingStartedEvent");
    }

    [Fact]
    public void Tick_WhenRunning_RaisesHeatingTickedEvent()
    {
        var session = HeatingSession.Start(BuildParameters(seconds: 5), Renderer).Value;
        session.ClearDomainEvents();

        session.Tick(Renderer);

        session.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "HeatingTickedEvent");
    }

    [Fact]
    public void Pause_RaisesHeatingPausedEvent()
    {
        var session = HeatingSession.Start(BuildParameters(), Renderer).Value;
        session.ClearDomainEvents();

        session.Pause();

        session.DomainEvents.Should().ContainSingle(e =>
            e.GetType().Name == "HeatingPausedEvent");
    }
}