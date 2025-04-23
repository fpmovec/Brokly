using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Brokly.Pipeline;

public class RequestPipelineWrapper<TRequest, TResponse>: IRequestPipelineWrapper<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandlePipelineAsync(
        TRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Task<TResponse> AsyncHandler(CancellationToken token = default) => serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .HandleAsync(request, token);

        var pipeLineConsumers = serviceProvider.GetServices<IRequestPipeline<TRequest, TResponse>>();

        return pipeLineConsumers
            .Aggregate((PipelineDelegate<TResponse>)AsyncHandler,
                (complete, consumer) => 
                    token => consumer.ExecuteAsync(request, complete, token == default ? cancellationToken : token))();
    }
    
}

public class RequestPipeLineWrapper<TRequest> : IRequestPipelineWrapper<TRequest>
    where TRequest : IRequest
{
    public Task HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Task AsyncHandler(CancellationToken token = default) => serviceProvider.GetRequiredService<IRequestHandler<TRequest>>()
            .HandleAsync(request, token);

        var pipeLineConsumers = serviceProvider.GetServices<IRequestPipeline<TRequest>>();

        return pipeLineConsumers
            .Aggregate((PipelineDelegate)AsyncHandler,
                (complete, consumer) => 
                    token => consumer.ExecuteAsync(request, complete, token == default ? cancellationToken : token))();
    }
}