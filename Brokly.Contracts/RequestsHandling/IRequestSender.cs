namespace Brokly.Contracts.RequestsHandling;

public interface IRequestSender
{
    Task<TResult> Send<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>;
    
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest;
}