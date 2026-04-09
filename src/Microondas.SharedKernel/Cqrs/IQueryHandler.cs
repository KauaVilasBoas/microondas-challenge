using MediatR;

namespace Microondas.SharedKernel.Cqrs;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
