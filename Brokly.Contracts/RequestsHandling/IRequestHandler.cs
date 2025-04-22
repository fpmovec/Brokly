namespace Brokly.Contracts.RequestsHandling;

/// <summary>
/// Defines a handler for a Brokly request with a response of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled, must implement <see cref="IRequest{TResult}"/>.</typeparam>
/// <typeparam name="TResult">The type of response from the handler.</typeparam>
/// <remarks>
/// Implement this interface to create request handlers for queries or commands that return results.
/// The handler contains the business logic to process the request and generate a response.
/// </remarks>
public interface IRequestHandler<in TRequest, TResult>
    where TRequest: IRequest<TResult>
{
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the response.</returns>
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a Brokly request without a response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled, must implement <see cref="IRequest"/>.</typeparam>
/// <remarks>
/// Implement this interface to create request handlers for commands that don't return results.
/// The handler contains the business logic to process the request.
/// </remarks>
public interface IRequestHandler<in TRequest>
    where TRequest: IRequest
{
    
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}