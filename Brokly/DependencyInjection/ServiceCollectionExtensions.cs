using System.Reflection;
using Brokly.Application.EventsHandling;
using Brokly.Application.Pipeline;
using Brokly.Application.RequestHandling;
using Brokly.Contracts.EventsHandling;
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.Processors;
using Brokly.Contracts.RequestsHandling;
using Brokly.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Brokly.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddBroklyRequestHandlersFromAssemblies(IServiceCollection services, Assembly[] assemblies)
    {
        var handlersWithResult = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .SelectMany(x => x.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                .Select(i => new { Interface = i, Implementation = x }))
            .ToList();

        foreach (var handler in handlersWithResult)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }
        
        return services;
    }

    private static IServiceCollection AddBroklyEventHandlersFromAssemblies(IServiceCollection services, Assembly[] assemblies)
    {
        var handlersWithResult = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .SelectMany(x => x.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .Select(i => new { Interface = i, Implementation = x }))
            .ToList();

        foreach (var handler in handlersWithResult)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
        
        return services;
    }
    
    /// <summary>
    /// Allows adding Brokly to the DI container with custom:
    /// </summary>
    /// <param name="options">represents the Action delegate that allows customizing Brokly options</param>
    public static IServiceCollection AddBrokly(this IServiceCollection services, Action<BroklyOptions> options)
    {
        var opts = new BroklyOptions();
        
        options.Invoke(opts);
        
        services.AddSingleton(opts);
        
        services.AddScoped<IRequestSender, RequestSender>();
        
        AddBroklyRequestHandlersFromAssemblies(services, [..opts.AssembliesToRegister]);

        if (opts.UseEventsHandling)
        {
            services.AddSingleton<IEventBus, EventBus>();
            
            AddBroklyEventHandlersFromAssemblies(services, [..opts.AssembliesToRegister]);
        }

        if (opts.UsePipelines)
        {
            services.AddSingleton<RequestsPipelines>();
        }
        
        return services;
    } 
    
    /// <summary>
    /// Allows adding Brokly to the DI container with default options:
    /// <code>MaxQueueSize = 500,
    /// ConsumersCount = 5,
    /// UseEventsHandling = false,
    /// AssembliesToRegister = *calling assembly*</code>
    /// </summary>
    public static IServiceCollection AddBrokly(this IServiceCollection services)
    {
        return AddBrokly(services, options =>
        {
            options.AddHandlersFromAssembly(Assembly.GetCallingAssembly());
        });
    } 
}