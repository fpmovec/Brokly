using Brokly.Contracts.Processors;

namespace Brokly.Application.Pipeline.Processors;

/// <summary>
/// Specifies a processor type to be used for handling requests.
/// </summary>
/// <remarks>
/// This attribute can be applied to classes to indicate which processor
/// should be used to handle the request. The processor type must implement
/// <see cref="IProcessor{TRequest}"/> or <see cref="IProcessor{TRequest, TResult}"/>.
/// </remarks>
/// <param name="processorHandlerType">The type of the processor to use for handling requests.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseProcessorAttribute(Type processorHandlerType) : Attribute, IProcessorAttribute
{
    /// <summary>
    /// Gets the type of the processor that will handle the request.
    /// </summary>
    /// <value>
    /// The <see cref="Type"/> of the processor that will process the request.
    /// This processor must implement either <see cref="IProcessor{TRequest}"/> or
    /// <see cref="IProcessor{TRequest, TResult}"/>.
    /// </value>
    public Type ProcessorHandlerType { get; init; } = processorHandlerType;
}