using MediatR;

namespace Microondas.Domain.Contracts.Queries;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
