using System.Reflection.Metadata;
using Brokly.Application.Pipeline.Processors;
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;

namespace ExampleApi;

public class WithResult
{
    public class SimpleRequest : IRequest<string>
    {
        public string Name { get; set; }
    }

}

public class WithoutResult
{
    public class SimpleRequest : IRequest
    {
        public string Name { get; set; }
    }

}



[UseProcessor(typeof(SimpleLoggingProcessor<WithResult.SimpleRequest, string>))]
[UseProcessor(typeof(TestProcessor<WithResult.SimpleRequest, string>))]
public class SimpleHandler(ILogger<SimpleHandler> logger) : IRequestHandler<WithResult.SimpleRequest, string>
{
    public Task<string> HandleAsync(WithResult.SimpleRequest request, CancellationToken cancellationToken)
    {
        logger.LogCritical($"Simple request: {request.Name}");
        return Task.FromResult($"{request.Name} is handled");
    }
}

public class LoggingPipeline<TRequest, TResult>(ILogger<LoggingPipeline<TRequest, TResult>> logger)
    : IRequestPipeline<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<TResult> ExecuteAsync(TRequest request, PipelineDelegate<TResult> complete, CancellationToken cancellationToken = default)
    {
        logger.LogWarning($"Input {typeof(TRequest).Name} {request.GetType().Name}");

        var result = await complete(cancellationToken);
        
        logger.LogWarning($"Output {typeof(TRequest).Name} {request.GetType().Name}");

        return result;
    }
}

public class SimplePipeline<TRequest> : IRequestPipeline<TRequest>
    where TRequest : IRequest
{
    public async Task ExecuteAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("First pipeline");

        await complete(cancellationToken);
    }
}

public class SimpleSecondPipeline<TRequest> : IRequestPipeline<TRequest>
    where TRequest : IRequest
{
    public Task ExecuteAsync(TRequest request, PipelineDelegate complete, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Second pipeline");
        
        return complete(cancellationToken);
    }
}

public class SimpleLoggingProcessor<TRequest, TResult>(ILogger<SimpleLoggingProcessor<TRequest, TResult>> logger)
    : ProcessorBase<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    protected override Task PreProcess(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogError("Pre process");
        
        return Task.CompletedTask;
    }

    protected override Task PostProcess(TResult request, CancellationToken cancellationToken)
    {
        logger.LogError("Post process");
        
        return Task.CompletedTask;
    }
    
}

public class TestProcessor<TRequest, TResult>(ILogger<TestProcessor<TRequest, TResult>> logger) : ProcessorBase<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    protected override Task PostProcess(TResult response, CancellationToken cancellationToken)
    {
        logger.LogWarning($"PostProcess TestProcessor {typeof(TRequest).Name}");
        
        return Task.CompletedTask;
    }

    protected override Task PreProcess(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning($"PreProcess TestProcessor {typeof(TResult).Name}");
        
        return Task.CompletedTask;
    }
    
}

public class SecondProcessorWithoutResult<TRequest>(ILogger<SecondProcessorWithoutResult<TRequest>> logger)
    : ProcessorBase<TRequest>
    where TRequest : IRequest
{
    protected override Task PreProcess(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("Second PRE process");
        
        return Task.CompletedTask;
    }

    protected override Task PostProcess(CancellationToken cancellationToken)
    {
        logger.LogWarning("Second POST process");
        
        return Task.CompletedTask;
    }
}

public class ProcessorWithoutResult(ILogger<ProcessorWithoutResult> logger)
    : ProcessorBase<WithoutResult.SimpleRequest>
{
    protected override Task PreProcess(WithoutResult.SimpleRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("Second PRE process");
        
        return Task.CompletedTask;
    }

    protected override Task PostProcess(CancellationToken cancellationToken)
    {
        logger.LogWarning("Second POST process");
        
        return Task.CompletedTask;
    }
}

[UseProcessor(typeof(SecondProcessorWithoutResult<WithoutResult.SimpleRequest>))]
[UseProcessor(typeof(ProcessorWithoutResult))]
public class SecondHandler(ILogger<SecondHandler> logger) : IRequestHandler<WithoutResult.SimpleRequest>
{
    public Task HandleAsync(WithoutResult.SimpleRequest request, CancellationToken cancellationToken)
    {
        logger.LogCritical($"Simple request: {request.Name}");
        return Task.FromResult($"{request.Name} is handled");
    }
}

public interface ISecondProcessor<in TRequest> : IProcessor<TRequest, string>
    where TRequest : IRequest<string>
{
    
}

public class SecondProcessor<TRequest> : ISecondProcessor<TRequest> where TRequest : IRequest<string>
{
    public Task<string> ProcessAsync(TRequest request, PipelineDelegate<string> complete, CancellationToken cancellationToken)
    {
        return complete(cancellationToken);
    }
}
