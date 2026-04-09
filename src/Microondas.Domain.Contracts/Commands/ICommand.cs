using MediatR;
using Microondas.SharedKernel;

namespace Microondas.Domain.Contracts.Commands;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}