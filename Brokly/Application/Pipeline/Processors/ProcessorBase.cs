using Brokly.Contracts.Pipeline;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Application.Pipeline.Processors;

public abstract class ProcessorBase<TRequest, TResult> : IProcessor<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    protected virtual Task PreProcess(TRequest request, CancellationToken cancellationToken)
        => Task.CompletedTask;
    
    protected virtual Task PostProcess(TResult response, CancellationToken cancellationToken)
        => Task.CompletedTask;
    
    public async Task<TResult> ProcessAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken)
    {
        await PreProcess(request, cancellationToken).ConfigureAwait(false);
        
        var result = await complete(cancellationToken);
        
        await PostProcess(result, cancellationToken).ConfigureAwait(false);
        
        return result;
    }
}

public abstract class ProcessorBase<TRequest> : IProcessor<TRequest>
    where TRequest : IRequest
{
    protected virtual Task PreProcess(TRequest request, CancellationToken cancellationToken)
        => Task.CompletedTask;
    
    protected virtual Task PostProcess(CancellationToken cancellationToken)
        => Task.CompletedTask;
    
    public async Task ProcessAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken)
    {
        await PreProcess(request, cancellationToken).ConfigureAwait(false);
        
        await complete(cancellationToken);
        
        await PostProcess(cancellationToken).ConfigureAwait(false);
    }
}