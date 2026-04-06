using Microondas.Domain.Contracts.Queries;
using Microondas.Domain.Heating.ValueObjects;
using Microondas.Domain.Services;

namespace Microondas.Application.ReadModels.Heating;

public sealed class GetHeatingStatusQueryHandler : IQueryHandler<GetHeatingStatusQuery, HeatingStatusReadModel?>
{
    private readonly HeatingSessionHolder _sessionHolder;

    public GetHeatingStatusQueryHandler(HeatingSessionHolder sessionHolder) =>
        _sessionHolder = sessionHolder;

    public Task<HeatingStatusReadModel?> Handle(GetHeatingStatusQuery request, CancellationToken cancellationToken)
    {
        var session = _sessionHolder.Current;

        if (session is null)
            return Task.FromResult<HeatingStatusReadModel?>(null);

        var displayTime = HeatingDisplayTime.From(session.RemainingSeconds);

        var readModel = new HeatingStatusReadModel(
            Status: session.Status.ToString(),
            RemainingSeconds: session.RemainingSeconds,
            DisplayTime: displayTime.Formatted,
            ElapsedSeconds: session.ElapsedSeconds,
            CurrentOutput: session.CurrentOutput,
            PowerLevel: session.Parameters.Power.Value,
            IsProgramSession: session.IsProgramSession,
            ProgramId: session.ProgramId);

        return Task.FromResult<HeatingStatusReadModel?>(readModel);
    }
}
