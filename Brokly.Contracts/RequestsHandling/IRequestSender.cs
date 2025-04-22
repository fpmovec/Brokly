namespace Brokly.Contracts.RequestsHandling;

/// <summary>
/// Provides a service for sending Brokly requests and receiving responses.
/// </summary>
/// <remarks>
/// This interface abstracts the Brokly pipeline, allowing for request/response messaging
/// between components while maintaining loose coupling.
/// </remarks>
public interface IRequestSender
{
    
    /// <summary>
    /// Sends a request that expects a response of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request, must implement <see cref="IRequest{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">The type of the expected response.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the response.</returns>
    Task<TResult> Send<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>;
    
    /// <summary>
    /// Sends a request that does not expect a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request, must implement <see cref="IRequest"/>.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest;
}