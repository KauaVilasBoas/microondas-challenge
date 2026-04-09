using Microondas.SharedKernel.Cqrs;

namespace Microondas.Application.ReadModels.Heating;

public sealed record GetHeatingStatusQuery : IQuery<HeatingStatusReadModel?>;