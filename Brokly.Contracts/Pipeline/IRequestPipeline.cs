using Brokly.Contracts.EventsHandling;
using Brokly.Contracts.RequestsHandling;
using Brokly.Contracts.Shared;

namespace Brokly.Contracts.Pipeline;

public delegate Task<TResult> PipelineDelegate<TResult>(CancellationToken cancellationToken = default);

public delegate Task PipelineDelegate(CancellationToken cancellationToken = default);

public interface IRequestPipeline<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken = default);
}

public interface IRequestPipeline<in TRequest>
    where TRequest : IRequest
{
    Task ExecuteAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken = default);
}