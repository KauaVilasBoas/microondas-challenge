using MediatR;
using Microondas.Application.Commands.Heating.TickHeating;
using Microondas.Domain.Heating;
using Microondas.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microondas.Workers;

public sealed class HeatingTimerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HeatingSessionHolder _sessionHolder;
    private readonly ILogger<HeatingTimerService> _logger;

    public HeatingTimerService(
        IServiceScopeFactory scopeFactory,
        HeatingSessionHolder sessionHolder,
        ILogger<HeatingTimerService> logger)
    {
        _scopeFactory = scopeFactory;
        _sessionHolder = sessionHolder;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var session = _sessionHolder.Current;
            if (session is null || session.Status != HeatingStatus.Running) continue;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new TickHeatingCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during heating tick");
            }
        }
    }
}