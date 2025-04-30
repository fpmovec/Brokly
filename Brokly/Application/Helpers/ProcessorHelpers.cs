using System.Reflection;
using Brokly.Application.Pipeline.Processors;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Brokly.Application.Helpers;

internal static class ProcessorHelpers
{
    private static List<Type> GetProcessorTypesForHandler(Type handlerType)
        => handlerType.GetCustomAttributes<UseProcessorAttribute>()
            .Select(x => x.ProcessorHandlerType)
            .ToList();

    public static List<TProcessor> GetProcessorsForHandler<TProcessor, THandler>(IServiceProvider serviceProvider)
        where TProcessor : IProcessorBase
        where THandler : IRequestHandler
    {
        var allProcessors = serviceProvider.GetServices<TProcessor>();
        var handlerType = serviceProvider.GetRequiredService<THandler>().GetType();

        return GetProcessorTypesForHandler(handlerType)
            .Select(x =>
            {
                var processor = allProcessors.FirstOrDefault(p => p.GetType() == x);

                return processor ?? throw new InvalidOperationException($"Type {x} is not an IProcessor.");
            })
            .Reverse()
            .ToList();
    }
}