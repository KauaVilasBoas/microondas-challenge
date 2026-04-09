using Microondas.Domain.Contracts.Events;
using Microondas.Domain.Programs.Events;
using Microsoft.Extensions.Logging;

namespace Microondas.Application.EventHandlers.Programs;

public sealed class CustomProgramDeletedEventHandler : IDomainEventHandler<CustomProgramDeletedEvent>
{
    private readonly ILogger<CustomProgramDeletedEventHandler> _logger;

    public CustomProgramDeletedEventHandler(ILogger<CustomProgramDeletedEventHandler> logger) =>
        _logger = logger;

    public Task Handle(CustomProgramDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Custom program deleted: Id={ProgramId}, Name={ProgramName}",
            notification.ProgramId,
            notification.ProgramName);

        return Task.CompletedTask;
    }
}