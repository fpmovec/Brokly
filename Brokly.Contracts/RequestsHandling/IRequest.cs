namespace Brokly.Contracts.RequestsHandling;

/// <summary>
/// Marker interface for requests that return a value of type <typeparamref name="TResult"/>.
/// Represents a message that can be handled by a single handler and produces a result.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the request handler.</typeparam>
public interface IRequest<TResult> : IRequest;

/// <summary>
/// Marker interface for requests that don't return a value.
/// Represents a message that can be handled by a single handler.
/// </summary>
public interface IRequest;