using System.Collections.Concurrent;
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

namespace Brokly.Application.Pipeline;

internal class RequestsPipelines
{
    private readonly ConcurrentDictionary<Type, IRequestPipelineWrapper> _requestPipelines = [];

    public IRequestPipelineWrapper GetOrAddPipeline(IRequest request, Func<Type, IRequestPipelineWrapper> factory)
    {
        return _requestPipelines.GetOrAdd(request.GetType(), factory);
    }
}