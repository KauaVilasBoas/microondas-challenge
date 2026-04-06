using Microondas.Domain.Contracts.Queries;

namespace Microondas.Application.ReadModels.Heating;

public sealed record GetHeatingStatusQuery : IQuery<HeatingStatusReadModel?>;
