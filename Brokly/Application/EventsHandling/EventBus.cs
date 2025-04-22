using System.Threading.Channels;
using Brokly.Contracts.EventsHandling;
using Brokly.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Brokly.Application.EventsHandling;

/// <summary>
/// Represents the class that contains events queue implementation
/// </summary>
public class EventBus : IEventBus, IDisposable
{
    public EventBus(
        IServiceScopeFactory scopeFactory,
        ILogger<EventBus> logger,
        BroklyOptions broklyOptions)
    {
        _broklyOptions = broklyOptions;
        _scopeFactory = scopeFactory;
        _logger = logger;

        EventsQueue = Channel.CreateBounded<IEvent>(
            new BoundedChannelOptions(_broklyOptions.MaxQueueSize)
        {
            SingleReader = _broklyOptions.ConsumersCount is 1,
            SingleWriter = false
        });
        
        StartEventsProcessing();
    }

    private readonly BroklyOptions _broklyOptions;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventBus> _logger;
    
    private readonly List<Task> _processingTasks = [];
    
    private readonly Channel<IEvent> EventsQueue;

    private void StartEventsProcessing()
    {
        _processingTasks.AddRange(
            Enumerable.Range(0, _broklyOptions.ConsumersCount)
            .Select(_ => Task.Run(() => ProcessEventsAsync(_cancellationTokenSource.Token))));
    }
    
    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        await EventsQueue.Writer.WriteAsync(@event, cancellationToken);
    }

    private async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        await foreach (var @event in EventsQueue.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                
                var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = scope.ServiceProvider.GetServices(handlerType)
                    .Cast<IEventHandler>();

                await Task.WhenAll(handlers.Select(handler => 
                    handler.HandleAsync(@event, cancellationToken)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {EventType}", @event.GetType().Name);
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        EventsQueue.Writer.Complete();
        Task.WaitAll([.._processingTasks], TimeSpan.FromSeconds(5));
        _cancellationTokenSource.Dispose();
    }   
}