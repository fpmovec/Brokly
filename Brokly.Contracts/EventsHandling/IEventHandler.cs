namespace Brokly.Contracts.EventsHandling;

/// <summary>
/// The base non-generic interface for an event handler
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// The method that is used for the events handling
    /// </summary>
    /// <param name="event">represents a base event that need to be handled</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>Task object of the asynchronous operation</returns>
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken);
}

public interface IEventHandler<TEvent> : IEventHandler
    where TEvent : IEvent
{
    
}