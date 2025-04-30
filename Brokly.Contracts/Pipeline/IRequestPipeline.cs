using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Pipeline;

/// <summary>
/// Represents an asynchronous pipeline delegate that returns a result.
/// </summary>
/// <typeparam name="TResult">The type of result produced by the pipeline.</typeparam>
/// <param name="cancellationToken">Optional cancellation token to abort the operation.</param>
/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
public delegate Task<TResult> PipelineDelegate<TResult>(CancellationToken cancellationToken = default);

/// <summary>
/// Represents an asynchronous pipeline delegate that does not return a result.
/// </summary>
/// <param name="cancellationToken">Optional cancellation token to abort the operation.</param>
/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
public delegate Task PipelineDelegate(CancellationToken cancellationToken = default);

/// <summary>
/// Defines a request processing pipeline for requests that return results.
/// </summary>
/// <typeparam name="TRequest">The type of request to process, which must implement <see cref="IRequest{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of result produced by the pipeline.</typeparam>
/// <remarks>
/// Implement this interface to create custom processing pipelines that can execute middleware
/// and ultimately delegate to a terminal handler.
/// </remarks>
public interface IRequestPipeline<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Executes the request processing pipeline.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The terminal pipeline delegate to call after all middleware completes.</param>
    /// <param name="cancellationToken">Optional cancellation token to abort the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation with the result.</returns>
    Task<TResult> ExecuteAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a request processing pipeline for requests that do not return results.
/// </summary>
/// <typeparam name="TRequest">The type of request to process, which must implement <see cref="IRequest"/>.</typeparam>
/// <remarks>
/// Implement this interface to create custom processing pipelines that can execute middleware
/// and ultimately delegate to a terminal handler for void-returning requests.
/// </remarks>
public interface IRequestPipeline<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Executes the request processing pipeline.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="complete">The terminal pipeline delegate to call after all middleware completes.</param>
    /// <param name="cancellationToken">Optional cancellation token to abort the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExecuteAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken = default);
}