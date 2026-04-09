using MediatR;

namespace Microondas.Application.Commands.Behaviors;

public sealed class DomainEventDispatchBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMediator _mediator;
    private readonly DomainEventCollector _collector;

    public DomainEventDispatchBehavior(IMediator mediator, DomainEventCollector collector)
    {
        _mediator = mediator;
        _collector = collector;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        foreach (var domainEvent in _collector.DomainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);

        _collector.Clear();
        return response;
    }
}