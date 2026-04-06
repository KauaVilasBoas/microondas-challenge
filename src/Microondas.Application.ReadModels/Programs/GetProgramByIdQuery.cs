using Microondas.Domain.Contracts.Queries;

namespace Microondas.Application.ReadModels.Programs;

public sealed record GetProgramByIdQuery(Guid ProgramId) : IQuery<ProgramReadModel?>;
