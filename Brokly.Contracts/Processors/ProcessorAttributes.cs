namespace Brokly.Contracts.Processors;

public interface IPreProcessorAttribute
{
    Type ProcessorHandlerType { get; init; }
}

public interface IPostProcessorAttribute
{
    Type ProcessorHandlerType { get; init; }
}