namespace Brokly.Contracts.EventsHandling;

/// <summary>
/// Provides a mechanism for publishing events asynchronously.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Asynchronously publishes an event of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish. This type must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
    Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}