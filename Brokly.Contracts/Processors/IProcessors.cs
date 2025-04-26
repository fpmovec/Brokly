using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Processors;

public interface IProcessor<in TRequest, TResult> : IProcessorBase
    where TRequest : IRequest<TResult>
{
    Task<TResult> ProcessAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken);
}

public interface IProcessor<in TRequest> : IProcessorBase
    where TRequest : IRequest
{
    Task ProcessAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken);
}