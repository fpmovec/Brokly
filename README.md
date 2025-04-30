
![brokly](https://github.com/user-attachments/assets/a9261337-73b3-431c-b55a-cfa40335ab3d)



# Brokly

**Elevate your application messaging with Brokly** – a streamlined implementation of the mediator pattern that brings structure to your CQRS flows while maintaining the flexibility you need.

[![NuGet Version](https://img.shields.io/nuget/v/Brokly.svg)](https://www.nuget.org/packages/Brokly)  
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## Features

- **Request/Response** via `IRequest<T>` / `IRequestHandler<TRequest, TResult>` and `IRequestSender`  
- **Event Handling** via `IEvent` / `IEventHandler<TEvent>` and `IEventBus` with in‑memory, bounded, channel‑based queue
- **Request Pipeline**: Global middleware via `IRequestPipeline<TRequest, TResult>` / `IRequestPipeline<TRequest>`
- **Processors**: Handler-specific middleware via `IProcessor<TRequest, TResult>` / `IProcessor<TRequest>`  
- **Customizable** queue size, concurrency (consumer count), and event‑handling toggle via `BroklyOptions`

## Installation

Install Brokly from NuGet:

```bash
dotnet add package Brokly --version <version_number>
```

This will automatically pull in the `Brokly.Contracts` package.

## Quick Start

### Register with Dependency Injection

In your `Program.cs` or `Startup.cs`:

```csharp
using Brokly.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Default registration (scans calling assembly, no events/pipelines/processors handling)
builder.Services.AddBrokly();

// OR with customization:
builder.Services.AddBrokly(options =>
{
    options
        .AddHandlersFromAssembly(typeof(Program).Assembly)
        .UseEventHandling(true)
        .UseRequestPipelines() 
        .SetMaxQueueSize(1000)
        .AddConsumers(3);
});

var app = builder.Build();
```

### Define a Request & Handler

```csharp
using Brokly.Contracts.RequestsHandling;

// Define the request
public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

// Implement the handler
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public Task<UserDto> HandleAsync(GetUserByIdQuery request, CancellationToken ct)
    {
        // ... load user ...
        return Task.FromResult(fetchedUser);
    }
}
```

### Send a Request

Inject `IRequestSender` and send:

```csharp
public class UsersController : ControllerBase
{
    private readonly IRequestSender _sender;

    public UsersController(IRequestSender sender)
        => _sender = sender;

    [HttpGet("{id}")]
    public async Task<UserDto> Get(Guid id, CancellationToken ct)
        => await _sender.Send<GetUserByIdQuery, UserDto>(new(id), ct);
}
```

### Define an Event & Handler

```csharp
using Brokly.Contracts.EventsHandling;

// Define the event
public record UserCreatedEvent(UserDto User) : IEvent;

// Implement the handler
public class SendWelcomeEmailHandler : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent @event, CancellationToken ct)
    {
        // ... send email ...
        return Task.CompletedTask;
    }
}
```

### Publish an Event

Inject `IEventBus` and publish:

```csharp
public class UserService
{
    private readonly IEventBus _events;

    public UserService(IEventBus events) => _events = events;

    public async Task CreateUserAsync(UserDto dto, CancellationToken ct)
    {
        // ... create user ...
        await _events.PublishEventAsync(new UserCreatedEvent(dto), ct);
    }
}
```
## Request Pipeline

Brokly’s request pipelines let you apply global middleware to **all** requests, with or without return values.

### Pipeline with Result

Implement `IRequestPipeline<TRequest, TResult>`:

```csharp
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

public class LoggingPipeline<TRequest, TResult> : IRequestPipeline<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    public async Task<TResult> ExecuteAsync(
        TRequest request,
        PipelineDelegate<TResult> complete,
        CancellationToken ct)
    {
        Console.WriteLine($"[Pipeline] Handling {typeof(TRequest).Name}");
        var result = await complete(ct);
        Console.WriteLine($"[Pipeline] Handled {typeof(TRequest).Name}");
        return result;
    }
}
```

### Pipeline without Result

Implement `IRequestPipeline<TRequest>`:

```csharp
using Brokly.Contracts.Pipeline;
using Brokly.Contracts.RequestsHandling;

public class AuditPipeline<TRequest> : IRequestPipeline<TRequest>
    where TRequest : IRequest
{
    public async Task ExecuteAsync(
        TRequest request,
        PipelineDelegate complete,
        CancellationToken ct)
    {
        Console.WriteLine($"[Pipeline] Auditing {typeof(TRequest).Name}");
        await complete(ct);
        Console.WriteLine($"[Pipeline] Audit complete for {typeof(TRequest).Name}");
    }
}
```

### Register Pipelines

Register in DI:

```csharp
builder.Services.AddTransient(
    typeof(IRequestPipeline<,>),
    typeof(LoggingPipeline<,>));

builder.Services.Transient(
    typeof(IRequestPipeline<>),
    typeof(AuditPipeline<>));
```

### Execution Flow

1. All registered pipelines execute **before** the handler (in registration order)
2. The handler’s `HandleAsync` runs
3. Pipelines execute **after** the handler (in reverse order)


## Processors

Processors provide **per-handler** middleware via attributes, for commands and queries (with or without result).

### Processor without Result

Inherit from `ProcessorBase<TRequest>`:

```csharp
using Brokly.Application.Pipeline.Processors;
using Brokly.Contracts.RequestsHandling;

public class ValidationProcessor<TRequest> : ProcessorBase<TRequest>
    where TRequest : IRequest
{
    protected override Task PreProcess(TRequest request, CancellationToken ct)
    {
        Console.WriteLine($"[Processor] Validating {typeof(TRequest).Name}");
        // throw if invalid
        return Task.CompletedTask;
    }

    protected override Task PostProcess(CancellationToken ct)
    {
        Console.WriteLine($"[Processor] Validation complete");
        return Task.CompletedTask;
    }
}
```

### Processor with Result

Inherit from `ProcessorBase<TRequest, TResult>`:

```csharp
using Brokly.Application.Pipeline.Processors;
using Brokly.Contracts.RequestsHandling;

public class MetricsProcessor<TRequest, TResult> : ProcessorBase<TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    protected override Task PreProcess(TRequest request, CancellationToken ct)
    {
        Console.WriteLine($"[Processor] Starting metrics for {typeof(TRequest).Name}");
        return Task.CompletedTask;
    }

    protected override Task PostProcess(TResult response, CancellationToken ct)
    {
        Console.WriteLine($"[Processor] Metrics recorded for {typeof(TRequest).Name}");
        return Task.CompletedTask;
    }
}
```

### Apply to Handlers

Decorate handlers with `[UseProcessor]` (multiple supported):

```csharp
using Brokly.Application.Pipeline.Processors;

[UseProcessor(typeof(MetricsProcessor<CreateOrderCommand, OrderResult>))]
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    public Task<OrderResult> HandleAsync(CreateOrderCommand request, CancellationToken ct)
    {
        // handle order creation
    }
}
```
OR
```csharp
using Brokly.Application.Pipeline.Processors;

[UseProcessor(typeof(ValidationProcessor<CreateOrderCommand>))]
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
{
    public Task HandleAsync(CreateOrderCommand request, CancellationToken ct)
    {
        // handle order creation
    }
}
```

### Register Processors

Register explicitly or bulk-scan:

```csharp
builder.Services.AddTransient<
    IProcessor<CreateOrderCommand>,
    ValidationProcessor<CreateOrderCommand>>();

builder.Services.AddTransient<
    IProcessor<CreateOrderCommand, OrderResult>,
    MetricsProcessor<CreateOrderCommand, OrderResult>>();
```

### Execution Flow per Handler

1. `PreProcess` on each processor (in attribute/declaration order)
2. Handler’s `HandleAsync`
3. `PostProcess` on each processor (in reverse order)


## Configuration Options

Configure `BroklyOptions` in the `AddBrokly` lambda:

- `AddHandlersFromAssembly(Assembly)` / `AddHandlersFromAssemblies(Assembly[])`  
- `UseEventHandling(bool)` – enable or disable event bus
- `UseRequestPipelines(bool)` – toggle request pipelines
- `SetMaxQueueSize(int)` – maximum in‑flight events in the channel queue  
- `AddConsumers(int)` – number of parallel event‑processing workers
  If `consumers count is 1`, FIFO approach for events handling is guaranteed, else it's not. The more consumers there are, the faster events are handled

See the full `BroklyOptions` API in the [Domain project](https://github.com/fpmovec/Brokly/tree/main/Brokly/Domain).


## Contributing

1. Fork the repository  
2. Create your feature branch (`git checkout -b feature/YourFeature`)  
3. Commit your changes (`git commit -m 'Add some feature'`)  
4. Push to the branch (`git push origin feature/YourFeature`)  
5. Open a Pull Request  

Please follow the existing code style and include tests where appropriate.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
