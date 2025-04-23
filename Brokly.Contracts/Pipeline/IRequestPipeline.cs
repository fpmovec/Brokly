using Brokly.Contracts.EventsHandling;
using Brokly.Contracts.RequestsHandling;
using Brokly.Contracts.Shared;

namespace Brokly.Contracts.Pipeline;

public delegate Task<TResponse> PipelineDelegate<TResponse>(CancellationToken cancellationToken = default);

public delegate Task PipelineDelegate(CancellationToken cancellationToken = default);

public interface IRequestPipeline<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request, PipelineDelegate<TResponse> complete, CancellationToken cancellationToken = default);
}

public interface IRequestPipeline<in TRequest>
    where TRequest : IRequest
{
    Task ExecuteAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken = default);
}