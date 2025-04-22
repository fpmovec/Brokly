namespace Brokly.Contracts.RequestsHandling;

public interface ICommand : IRequest;

public interface ICommand<TResult> : IRequest<TResult>;