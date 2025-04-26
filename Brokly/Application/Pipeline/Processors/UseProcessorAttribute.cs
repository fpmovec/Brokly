using Brokly.Contracts.Processors;

namespace Brokly.Application.Pipeline.Processors;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseProcessorAttribute(Type processorHandlerType) : Attribute, IPostProcessorAttribute
{
    public Type ProcessorHandlerType { get; init; } = processorHandlerType;
}