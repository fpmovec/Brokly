using System.Reflection;

namespace Brokly.Domain;


public class BroklyOptions
{
    /// <summary>
    /// Assemblies those handlers are placed in
    /// </summary>
    public List<Assembly> AssembliesToRegister { get; } = [];
    
    /// <summary>
    /// The maximum number of events those can be placed in the queue at the same time
    /// </summary>
    public int MaxQueueSize { get; private set; } = 500;
    
    /// <summary>
    /// Number thar shows how many 'consumers' handle events
    /// </summary>
    /// /// <remarks>
    /// If <c>consumers count is 1</c>, FIFO approach for events handling is guaranteed, else is not.
    /// The more consumers there are, the faster events are handled
    /// </remarks>
    public int ConsumersCount { get; private set; } = 5;
    
    /// <summary>
    /// Shows whether event handling is used
    /// </summary>
    public bool UseEventsHandling { get; private set; }

    /// <summary>
    /// Allows adding handlers to the DI container from the specified assembly
    /// </summary>
    /// <param name="assembly">assembly that handlers are placed in</param>
    public BroklyOptions AddHandlersFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);

        return this;
    }
    
    /// <summary>
    /// Allows adding handlers to the DI container from the specified assemblies
    /// </summary>
    /// <param name="assemblies">assemblies those handlers are placed in</param>
    public BroklyOptions AddHandlersFromAssemblies(Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);

        return this;
    }

    /// <summary>
    /// Allows setting the maximum number of events those can be placed in the queue at the same time
    /// </summary>
    /// <param name="maxEventsForQueue">maximum queue size</param>
    /// <returns></returns>
    public BroklyOptions SetMaxQueueSize(int maxEventsForQueue)
    {
        MaxQueueSize = maxEventsForQueue;

        return this;
    }
    
    /// <summary>
    /// Allows adding handlers to the DI container from the assembly that contains <c>TMarker</c> type
    /// </summary>
    /// <typeparam name="TMarker">the type contained in the assembly from which we register handlers</typeparam>
    public BroklyOptions AddHandlersFromAssemblyOf<TMarker>()
    {
        return AddHandlersFromAssembly(typeof(TMarker).Assembly);
    }

    /// <summary>
    /// Allows specifying whether events handling must be added
    /// </summary>
    /// <param name="useEventHandling">shows whether event handling is used</param>
    public BroklyOptions UseEventHandling(bool useEventHandling = true)
    {
        UseEventsHandling = useEventHandling;

        return this;
    }

    /// <summary>
    /// Allows setting how many 'consumers' handle events
    /// </summary>
    /// <param name="count">consumers count</param>
    /// <remarks>
    /// If <c>consumers count is 1</c>, FIFO approach for events handling is guaranteed, else it's not.
    /// The more consumers there are, the faster events are handled
    /// </remarks>
    public BroklyOptions AddConsumers(int count)
    {
        ConsumersCount = count;
        
        return this;
    }
}