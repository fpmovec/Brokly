using System.Reflection;

namespace Brokly.Domain;


public class BroklyOptions
{
    public List<Assembly> AssembliesToRegister { get; } = [];
    public int MaxQueueSize { get; private set; } = 500;
    public int ConsumersCount { get; private set; } = 5;
    public bool UseEventsHandling { get; private set; }

    public BroklyOptions AddHandlersFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);

        return this;
    }
    
    public BroklyOptions AddHandlersFromAssemblies(Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);

        return this;
    }

    public BroklyOptions SetMaxQueueSize(int maxEventsForQueue)
    {
        MaxQueueSize = maxEventsForQueue;

        return this;
    }
    
    public BroklyOptions AddHandlersFromAssemblyOf<TMarker>()
    {
        return AddHandlersFromAssembly(typeof(TMarker).Assembly);
    }

    public BroklyOptions UseEventHandling(bool useEventHandling = true)
    {
        UseEventsHandling = useEventHandling;

        return this;
    }

    public BroklyOptions AddConsumers(int count)
    {
        ConsumersCount = count;
        
        return this;
    }
}