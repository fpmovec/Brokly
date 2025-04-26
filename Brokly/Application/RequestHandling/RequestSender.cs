using Brokly.Application.Pipeline;
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Brokly.Application.RequestHandling;

/// <summary>
/// Represents the class that contains request handling implementation
/// </summary>
public class RequestSender(
    ILogger<RequestSender> logger,
    IServiceProvider serviceProvider,
    RequestsPipelines pipelines) : IRequestSender
{
    public Task<TResult> Send<TRequest, TResult>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResult>
    {
        try
        {
            var requestPipeline = (RequestPipelineWrapper<TRequest, TResult>)pipelines.GetOrAddPipeline(
                request,
                requestType =>
                {
                    var pipelineWrapperType = typeof(RequestPipelineWrapper<,>).MakeGenericType(requestType, typeof(TResult));

                    if (Activator.CreateInstance(pipelineWrapperType) is not IRequestPipelineWrapper wrapper)
                    {
                        throw new InvalidOperationException($"Unable to create instance of {pipelineWrapperType}");
                    }
                    
                    return wrapper;
                });

            return  requestPipeline.HandlePipelineAsync(request, serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            throw;
        }
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest
    {
        try
        {
            var pipeline = (RequestPipelineWrapper<TRequest>)pipelines.GetOrAddPipeline(
                request,
                requestType =>
                {
                    var pipelineWrapperType = typeof(RequestPipelineWrapper<>).MakeGenericType(requestType);

                    if (Activator.CreateInstance(pipelineWrapperType) is not IRequestPipelineWrapper wrapper)
                    {
                        throw new InvalidOperationException($"Unable to create instance of {pipelineWrapperType}");
                    }

                    return wrapper;
                });
            
            await pipeline.HandlePipelineAsync(request, serviceProvider, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            throw;
        }
    }
}