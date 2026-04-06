using MediatR;

namespace Microondas.Domain.Contracts.Queries;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
