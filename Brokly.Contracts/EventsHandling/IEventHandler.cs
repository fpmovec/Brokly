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
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken);
}

public interface IEventHandler<in TEvent> : IEventHandler
    where TEvent : IEvent
{
    /// <summary>
    /// The method that is used for the events handling
    /// </summary>
    /// <param name="event">represents a specific event that need to be handled</param>
    /// <param name="cancellationToken">cancellation token</param>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    
    Task IEventHandler.HandleAsync(IEvent @event, CancellationToken cancellationToken)
    {
        return HandleAsync((TEvent)@event, cancellationToken);
    }
}