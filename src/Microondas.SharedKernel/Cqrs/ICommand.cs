using MediatR;

namespace Microondas.SharedKernel.Cqrs;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
