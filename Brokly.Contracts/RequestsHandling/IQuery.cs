namespace Brokly.Contracts.RequestsHandling;

/// <summary>
/// Represents a query that returns a result of type <typeparamref name="TResult"/>.
/// Queries are used to read data from the system without modifying state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query handler.</typeparam>
public interface IQuery<TResult> : IRequest<TResult>;