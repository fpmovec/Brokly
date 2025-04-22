namespace Brokly.Contracts.RequestsHandling;

/// <summary>
/// Represents a command that does not return a result.
/// Commands are used to modify state in the system.
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Represents a command that returns a result of type <typeparamref name="TResult"/>.
/// Commands are used to modify state in the system and optionally return data.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command handler.</typeparam>
public interface ICommand<TResult> : IRequest<TResult>;