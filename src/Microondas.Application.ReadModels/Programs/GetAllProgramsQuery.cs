using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.ReadModels.Programs;

public sealed record GetAllProgramsQuery : IQuery<IReadOnlyList<ProgramReadModel>>;