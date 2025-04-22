namespace Brokly.Contracts.EventsHandling;

public interface IEventBus
{
    Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}