using Brokly.Contracts.Pipeline;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Application.Pipeline.Processors;

/// <summary>
/// Abstract base class for processors that handle requests and produce results.
/// </summary>
/// <typeparam name="TRequest">The type of the request, which must implement <see cref="IRequest{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the processor.</typeparam>
public abstract class ProcessorBase<TRequest, TResult> : IProcessor<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Called before the main processing occurs. Override to add pre-processing logic.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task PreProcess(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Called after the main processing completes. Override to add post-processing logic.
    /// </summary>
    /// <param name="response">The result produced by the processor.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task PostProcess(TResult response, CancellationToken cancellationToken);
    
    /// <summary>
    /// Processes the request by executing pre-processing, the main pipeline, and post-processing in sequence.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The pipeline delegate that handles the main processing.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation and contains the result.</returns>
    public async Task<TResult> ProcessAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken)
    {
        await PreProcess(request, cancellationToken).ConfigureAwait(false);
        
        var result = await complete(cancellationToken);
        
        await PostProcess(result, cancellationToken).ConfigureAwait(false);
        
        return result;
    }
}

/// <summary>
/// Abstract base class for processors that handle requests without producing results.
/// </summary>
/// <typeparam name="TRequest">The type of the request, which must implement <see cref="IRequest"/>.</typeparam>
public abstract class ProcessorBase<TRequest> : IProcessor<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Called before the main processing occurs. Override to add pre-processing logic.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task PreProcess(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Called after the main processing completes. Override to add post-processing logic.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task PostProcess(CancellationToken cancellationToken);
    
    /// <summary>
    /// Processes the request by executing pre-processing, the main pipeline, and post-processing in sequence.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The pipeline delegate that handles the main processing.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task ProcessAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken)
    {
        await PreProcess(request, cancellationToken).ConfigureAwait(false);
        
        await complete(cancellationToken);
        
        await PostProcess(cancellationToken).ConfigureAwait(false);
    }
}