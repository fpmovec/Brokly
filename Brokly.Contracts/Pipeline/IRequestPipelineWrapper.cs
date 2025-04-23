using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Pipeline;

public interface IRequestPipelineWrapper<in TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public interface IRequestPipelineWrapper<in TRequest>
    where TRequest : IRequest
{
    Task HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}