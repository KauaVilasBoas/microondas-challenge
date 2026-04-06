using Microondas.Domain.Contracts.Queries;
using Microondas.Domain.Contracts.Repositories;
using Microondas.Domain.Programs;

namespace Microondas.Application.ReadModels.Programs;

public sealed class GetProgramByIdQueryHandler : IQueryHandler<GetProgramByIdQuery, ProgramReadModel?>
{
    private readonly IHeatingProgramRepository _programRepository;

    public GetProgramByIdQueryHandler(IHeatingProgramRepository programRepository) =>
        _programRepository = programRepository;

    public async Task<ProgramReadModel?> Handle(GetProgramByIdQuery request, CancellationToken cancellationToken)
    {
        var predefined = PredefinedProgramSeed.GetAll()
            .FirstOrDefault(p => p.Id == request.ProgramId);

        if (predefined is not null) return MapToReadModel(predefined);

        var custom = await _programRepository.GetByIdAsync<HeatingProgram>(request.ProgramId, cancellationToken);
        return custom is null ? null : MapToReadModel(custom);
    }

    private static ProgramReadModel MapToReadModel(HeatingProgram program) =>
        new(
            Id: program.Id,
            Name: program.Name.Value,
            Food: program.Food.Value,
            TimeInSeconds: program.Time.TotalSeconds,
            Power: program.Power.Value,
            HeatingChar: program.Character.Value,
            Type: program.Type.ToString(),
            IsCustom: program.IsCustom,
            Instructions: program.Instructions.Value);
}
