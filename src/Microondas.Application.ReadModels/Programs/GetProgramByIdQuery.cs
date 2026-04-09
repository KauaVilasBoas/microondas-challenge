using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.ReadModels.Programs;

public sealed record GetProgramByIdQuery(Guid ProgramId) : IQuery<ProgramReadModel?>;