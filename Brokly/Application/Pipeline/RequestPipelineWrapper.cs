using System.Reflection;
using Brokly.Application.Helpers;
using Brokly.Application.Pipeline.Processors;
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Brokly.Application.Pipeline;

internal class RequestPipelineWrapper<TRequest, TResult> : IRequestPipelineWrapper<TRequest, TResult>
where TRequest : IRequest<TResult>
{
    public Task<TResult> HandlePipelineAsync(
        TRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var pipeLineConsumers = serviceProvider
            .GetServices<IRequestPipeline<TRequest, TResult>>()
            .Reverse();

        var processors = ProcessorHelpers
            .GetProcessorsForHandler<IProcessor<TRequest, TResult>, IRequestHandler<TRequest, TResult>>(serviceProvider);
        
        var processorChain = processors
            .Aggregate((PipelineDelegate<TResult>)AsyncHandler,
                (complete, processor) =>
                    token => processor.ProcessAsync(request, complete, token));
            
        return pipeLineConsumers
            .Aggregate(processorChain,
                (complete, consumer) => 
                    token => consumer.ExecuteAsync(request, complete, token == default ? cancellationToken : token))();
        
        Task<TResult> AsyncHandler(CancellationToken token = default)
            => serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>()
            .HandleAsync(request, token == default ? cancellationToken : token);
    }
}

public class RequestPipelineWrapper<TRequest> : IRequestPipelineWrapper<TRequest>
    where TRequest : IRequest
{
    public Task HandlePipelineAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var pipeLineConsumers = serviceProvider
            .GetServices<IRequestPipeline<TRequest>>()
            .Reverse();

        var processors = ProcessorHelpers
            .GetProcessorsForHandler<IProcessor<TRequest>, IRequestHandler<TRequest>>(serviceProvider);
        
        var processorChain = processors
            .Aggregate((PipelineDelegate)AsyncHandler,
                (complete, processor) =>
                    token => processor.ProcessAsync(request, complete, token));
        
        return pipeLineConsumers
            .Aggregate(processorChain,
                (complete, consumer) => 
                    token => consumer.ExecuteAsync(request, complete, token == default ? cancellationToken : token))();

        Task AsyncHandler(CancellationToken token = default)
            => serviceProvider.GetRequiredService<IRequestHandler<TRequest>>()
            .HandleAsync(request, token);
    }
}