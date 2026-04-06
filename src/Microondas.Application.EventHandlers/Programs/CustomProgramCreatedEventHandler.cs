using Microsoft.Extensions.Logging;
using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Programs.Events;

namespace Microondas.Application.EventHandlers.Programs;

public sealed class CustomProgramCreatedEventHandler : IDomainEventHandler<CustomProgramCreatedEvent>
{
    private readonly ILogger<CustomProgramCreatedEventHandler> _logger;

    public CustomProgramCreatedEventHandler(ILogger<CustomProgramCreatedEventHandler> logger) =>
        _logger = logger;

    public Task Handle(CustomProgramCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Custom program created: Id={ProgramId}, Name={ProgramName}",
            notification.ProgramId,
            notification.ProgramName);

        return Task.CompletedTask;
    }
}
