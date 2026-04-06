using Microondas.Domain.Contracts.Queries;

namespace Microondas.Application.ReadModels.Programs;

public sealed record GetAllProgramsQuery : IQuery<IReadOnlyList<ProgramReadModel>>;
