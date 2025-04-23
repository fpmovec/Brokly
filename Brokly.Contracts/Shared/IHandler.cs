using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Shared;

public interface IHandler<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
    
}

public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the response.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}