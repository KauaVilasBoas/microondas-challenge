using Microondas.SharedKernel.Cqrs;
using Microondas.Domain.Repositories;
using Microondas.Domain.Programs;

namespace Microondas.Application.ReadModels.Programs;

public sealed class GetAllProgramsQueryHandler : IQueryHandler<GetAllProgramsQuery, IReadOnlyList<ProgramReadModel>>
{
    private readonly IHeatingProgramRepository _programRepository;

    public GetAllProgramsQueryHandler(IHeatingProgramRepository programRepository) =>
        _programRepository = programRepository;

    public async Task<IReadOnlyList<ProgramReadModel>> Handle(
        GetAllProgramsQuery request,
        CancellationToken cancellationToken)
    {
        var persistedCustom = await _programRepository
            .GetAllAsync<HeatingProgram>(cancellationToken);

        var predefined = PredefinedProgramSeed.GetAll()
            .Select(MapToReadModel);

        var custom = persistedCustom
            .Where(p => p.IsCustom)
            .Select(MapToReadModel);

        return predefined.Concat(custom).ToList().AsReadOnly();
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