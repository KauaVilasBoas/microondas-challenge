using FluentValidation;
using MediatR;
using Microondas.SharedKernel;

namespace Microondas.Application.Commands.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next();

        var errors = failures
            .Select(f => $"{f.PropertyName}: {f.ErrorMessage}")
            .ToList();

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var errorType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result<>).MakeGenericType(errorType)
                .GetMethod(nameof(Result<object>.Failure))!;
            var error = Error.Validation("Validation.Failed", string.Join("; ", errors));
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            var error = Error.Validation("Validation.Failed", string.Join("; ", errors));
            return (TResponse)(object)Result.Failure(error);
        }

        throw new ValidationException(failures);
    }
}
