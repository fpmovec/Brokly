using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Processors;

/// <summary>
/// Represents a processor that handles requests and produces results.
/// </summary>
/// <typeparam name="TRequest">The type of request to process, which must implement <see cref="IRequest{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of result produced by the processor.</typeparam>
/// <remarks>
/// Implement this interface to create processors that handle specific request types and return results.
/// The processor typically executes pre-processing logic, delegates to the pipeline, and handles post-processing.
/// </remarks>
public interface IProcessor<in TRequest, TResult> : IProcessorBase
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Processes the request asynchronously.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The pipeline delegate to call for the main processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation and contains the result.</returns>
    Task<TResult> ProcessAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a processor that handles requests without producing results.
/// </summary>
/// <typeparam name="TRequest">The type of request to process, which must implement <see cref="IRequest"/>.</typeparam>
/// <remarks>
/// Implement this interface to create processors that handle specific request types without returning results.
/// The processor typically executes pre-processing logic, delegates to the pipeline, and handles post-processing.
/// </remarks>
public interface IProcessor<in TRequest> : IProcessorBase
    where TRequest : IRequest
{
    /// <summary>
    /// Processes the request asynchronously.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The pipeline delegate to call for the main processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task ProcessAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken);
}