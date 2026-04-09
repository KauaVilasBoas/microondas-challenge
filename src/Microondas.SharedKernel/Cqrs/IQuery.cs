using MediatR;

namespace Microondas.SharedKernel.Cqrs;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}
