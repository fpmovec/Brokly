using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Application.Pipeline.SpecificPipelines;

public abstract class PreProcessorPipeline<TRequest, TResult> : IRequestPipeline<TRequest, TResult>
where TRequest : IRequest<TResult>
{
    public abstract Task ProcessAsync(TRequest request, CancellationToken cancellationToken);
    public async Task<TResult> ExecuteAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken = default)
    {
        await ProcessAsync(request, cancellationToken);
        
        return await complete(cancellationToken);
    }
}



