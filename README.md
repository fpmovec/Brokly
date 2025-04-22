
![brokly](https://github.com/user-attachments/assets/a9261337-73b3-431c-b55a-cfa40335ab3d)



# Brokly

**Elevate your application messaging with Brokly** – a streamlined implementation of the mediator pattern that brings structure to your CQRS flows while maintaining the flexibility you need.

[![NuGet Version](https://img.shields.io/nuget/v/Brokly.svg)](https://www.nuget.org/packages/Brokly)  
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## Features

- **Request/Response** via `IRequest<T>` / `IRequestHandler<TRequest, TResult>` and `IRequestSender`  
- **Event Handling** via `IEvent` / `IEventHandler<TEvent>` and `IEventBus` with in‑memory, bounded, channel‑based queue  
- **Auto‑registration** of handlers in your DI container using `services.AddBrokly()` or `services.AddBrokly(opts => { … })`
- **Customizable** queue size, concurrency (consumer count), and event‑handling toggle via `BroklyOptions`

## Installation

Install Brokly from NuGet:

```bash
dotnet add package Brokly --version 1.0.1
```

This will automatically pull in the `Brokly.Contracts` package.

## Quick Start

### Register with Dependency Injection

In your `Program.cs` or `Startup.cs`:

```csharp
using Brokly.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Default registration (scans calling assembly, no events handling)
builder.Services.AddBrokly();

// OR with customization:
builder.Services.AddBrokly(options =>
{
    options
        .AddHandlersFromAssembly(typeof(Program).Assembly)
        .UseEventHandling(true)
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

## Configuration Options

Configure `BroklyOptions` in the `AddBrokly` lambda:

- `AddHandlersFromAssembly(Assembly)` / `AddHandlersFromAssemblies(Assembly[])`  
- `UseEventHandling(bool)` – enable or disable event bus  
- `SetMaxQueueSize(int)` – maximum in‑flight events in the channel queue  
- `AddConsumers(int)` – number of parallel event‑processing workers  

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
