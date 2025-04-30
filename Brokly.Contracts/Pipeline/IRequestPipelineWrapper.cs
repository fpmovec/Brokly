using Brokly.Contracts.RequestsHandling;

namespace Brokly.Contracts.Pipeline;

public interface IRequestPipelineWrapper;
public interface IRequestPipelineWrapper<in TRequest, TResult>: IRequestPipelineWrapper
where TRequest : IRequest<TResult>
{
    Task<TResult> HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public interface IRequestPipelineWrapper<in TRequest>: IRequestPipelineWrapper
    where TRequest : IRequest
{
    Task HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

