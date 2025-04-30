namespace Brokly.Contracts.Processors;

/// <summary>
/// Marker interface for attributes that specify a processor type to handle requests.
/// </summary>
/// <remarks>
/// Implement this interface on attributes that need to identify which processor
/// should be used to process a request. The processor type should implement
/// either <see cref="IProcessor{TRequest}"/> or <see cref="IProcessor{TRequest, TResult}"/>.
/// </remarks>
public interface IProcessorAttribute
{
    /// <summary>
    /// Gets the type of the processor that will handle the request.
    /// </summary>
    /// <value>
    /// The <see cref="Type"/> of the processor that will process the request.
    /// This should be a type that implements either <see cref="IProcessor{TRequest}"/>
    /// or <see cref="IProcessor{TRequest, TResult}"/>.
    /// </value>
    Type ProcessorHandlerType { get; }
}