# MassTransit Documentation

MassTransit is a messaging framework for building reliable and scalable distributed applications on .NET. It provides a unified abstraction layer over multiple message brokers including RabbitMQ, Azure Service Bus, Amazon SQS, and SQL Transport.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Message Contracts](#message-contracts)
3. [Consumers](#consumers)
4. [Configuration](#configuration)
5. [Transports](#transports)
6. [Publishing and Sending](#publishing-and-sending)
7. [Advanced Topics](#advanced-topics)

---

## Getting Started

### Installation

Install the MassTransit templates to get started quickly:

```bash
dotnet new install MassTransit.Templates
```

### Create a New Project

```bash
# Create a worker service
$ dotnet new mtworker -n GettingStarted
$ cd GettingStarted

# Add a consumer
$ dotnet new mtconsumer
```

### Quick Start Example

#### 1. Create a Message Contract

Create a `Contracts` folder and add a message contract:

```csharp
// Contracts/GettingStarted.cs
namespace GettingStarted.Contracts;

public record GettingStarted
{
    public string Value { get; init; }
}
```

#### 2. Create a Consumer

Create a `Consumers` folder and add a consumer:

```csharp
// Consumers/GettingStartedConsumer.cs
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

public class GettingStartedConsumer : IConsumer<GettingStarted>
{
    readonly ILogger<GettingStartedConsumer> _logger;

    public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<GettingStarted> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.Value);
        return Task.CompletedTask;
    }
}
```

#### 3. Configure MassTransit in Program.cs

```csharp
using MassTransit;

services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

services.AddMassTransitHostedService();
```

#### 4. Publish Messages

```csharp
using MassTransit;

public class Worker : BackgroundService
{
    readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new GettingStarted 
            { 
                Value = $"The time is {DateTimeOffset.Now}" 
            }, stoppingToken);
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

---

## Message Contracts

In MassTransit, a message contract is defined **code first** by creating a .NET type. Messages can be defined using records, interfaces, or classes.

### Message Types

#### Records (Recommended)

```csharp
namespace Company.Application.Contracts;

public record UpdateCustomerAddress
{
    public Guid CommandId { get; init; }
    public DateTime Timestamp { get; init; }
    public string CustomerId { get; init; }
    public string HouseNumber { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string PostalCode { get; init; }
}
```

#### Interfaces

```csharp
namespace Company.Application.Contracts;

public interface UpdateCustomerAddress
{
    Guid CommandId { get; }
    DateTime Timestamp { get; }
    string CustomerId { get; }
    string HouseNumber { get; }
    string Street { get; }
    string City { get; }
    string State { get; }
    string PostalCode { get; }
}
```

#### Classes

```csharp
namespace Company.Application.Contracts;

public class UpdateCustomerAddress
{
    public Guid CommandId { get; set; }
    public DateTime Timestamp { get; set; }
    public string CustomerId { get; set; }
    public string HouseNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
}
```

> **Note:** Properties with `private set;` are not recommended as they are not serialized by default when using `System.Text.Json`.

### Commands vs Events

#### Commands

A command tells *a* service to do something, and typically a command should only be consumed by a single consumer.

- Use **verb-noun** sequence (tell style)
- Examples: `UpdateCustomerAddress`, `UpgradeCustomerAccount`, `SubmitOrder`

```csharp
// A command to submit an order
public record SubmitOrder
{
    public Guid OrderId { get; init; }
    public string ProductName { get; init; }
    public int Quantity { get; init; }
}
```

#### Events

An event signifies that something has happened. Events are published via `ConsumeContext`, `IPublishEndpoint`, or `IBus`.

- Use **noun-verb (past tense)** sequence
- Examples: `CustomerAddressUpdated`, `CustomerAccountUpgraded`, `OrderSubmitted`

```csharp
// An event indicating an order was submitted
public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public DateTime Timestamp { get; init; }
}
```

### Message Headers

MassTransit encapsulates every sent or published message in a message envelope with headers:

| Property | Type | Description |
|----------|------|-------------|
| MessageId | Auto | Generated for each message using `NewId.NextGuid` |
| CorrelationId | User | Assigned by the application to uniquely identify the operation |
| RequestId | Request | Assigned by the request client |
| InitiatorId | Auto | Assigned when publishing/sending from a consumer |
| ConversationId | Auto | Assigned when the first message is sent |
| SourceAddress | Auto | Where the message originated |
| DestinationAddress | Auto | Where the message was sent |
| ResponseAddress | Request | Where responses should be sent |
| FaultAddress | User | Where consumer faults should be sent |
| ExpirationTime | User | When the message should expire |
| Headers | User | Additional custom headers |

### Message Correlation

Messages are part of a conversation, and identifiers connect messages to that conversation.

#### Setting CorrelationId

```csharp
// Using SendContext
await endpoint.Send<SubmitOrder>(new { OrderId = InVar.Id }, 
    sendContext => sendContext.CorrelationId = context.Message.OrderId);

// Using message initializer
await endpoint.Send<SubmitOrder>(new 
{ 
    OrderId = context.Message.OrderId,
    __CorrelationId = context.Message.OrderId
});
```

#### Correlation Conventions

MassTransit includes several correlation conventions:

1. If the message implements `CorrelatedBy<Guid>` interface
2. If the message has a property named `CorrelationId`, `CommandId`, or `EventId` that is a `Guid`
3. Register a custom correlation provider:

```csharp
// Register correlation convention
GlobalTopology.Send.UseCorrelationId<SubmitOrder>(x => x.OrderId);
```

---

## Consumers

A consumer consumes one or more message types when configured on or connected to a receive endpoint.

### Message Consumer

The most common consumer type:

```csharp
public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        // Process the order
        await context.Publish<OrderSubmitted>(new 
        { 
            context.Message.OrderId 
        });
    }
}
```

### Batch Consumer

For high-volume scenarios, MassTransit supports batch consumers:

```csharp
public class OrderBatchConsumer : IConsumer<Batch<OrderAudit>>
{
    public async Task Consume(ConsumeContext<Batch<OrderAudit>> context)
    {
        for (int i = 0; i < context.Message.Length; i++)
        {
            var message = context.Message[i];
            // Process each message in the batch
        }
    }
}
```

### Consumer Definition

Consumer definitions specify consumer behavior for automatic configuration:

```csharp
public class SubmitOrderConsumerDefinition 
    : ConsumerDefinition<SubmitOrderConsumer>
{
    public SubmitOrderConsumerDefinition()
    {
        // Override the default endpoint name
        EndpointName = "ha-submit-order";
        
        // Limit concurrent messages
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
```

### Skipped Messages

When a consumer is removed or a message is mistakenly sent to a receive endpoint without a consumer, messages are moved to a `*_skipped` queue.

---

## Configuration

### ASP.NET Core Configuration

```csharp
using MassTransit;

public void ConfigureServices(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<SubmitOrderConsumer>();
        
        x.SetKebabCaseEndpointNameFormatter();
        
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            
            cfg.ConfigureEndpoints(context);
        });
    });

    services.AddMassTransitHostedService();
}
```

### Publishing from a Controller

```csharp
using MassTransit;

public class OrdersController : ControllerBase
{
    readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task Post([FromBody] OrderRequest request)
    {
        await _publishEndpoint.Publish(new SubmitOrder
        {
            OrderId = Guid.NewGuid(),
            ProductName = request.ProductName,
            Quantity = request.Quantity
        });
        
        return Ok();
    }
}
```

### Health Checks

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks();
    
    services.AddMassTransit(x =>
    {
        x.UsingRabbitMq();
    });
    
    services.AddMassTransitHostedService();
}

public void Configure(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });
        
        endpoints.MapHealthChecks("/health/live");
    });
}
```

---

## Transports

### In-Memory Transport

For development and testing:

```bash
dotnet add package MassTransit
```

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
    
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
```

### RabbitMQ

```bash
dotnet add package MassTransit.RabbitMQ
```

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

Run RabbitMQ with Docker:
```bash
docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
```

### Azure Service Bus

```bash
dotnet add package MassTransit.Azure.ServiceBus.Core
```

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
    
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host("your-connection-string");
        cfg.ConfigureEndpoints(context);
    });
});
```

Requirements:
- Azure Service Bus namespace (Standard or Premium tier)
- Shared access policy with **Manage** permissions

### Amazon SQS

```bash
dotnet add package MassTransit.AmazonSqs
```

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
    
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("region", h =>
        {
            h.AccessKey("access-key");
            h.SecretKey("secret-key");
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

---

## Publishing and Sending

### Publishing Events

Use `IPublishEndpoint` to publish events:

```csharp
public class OrderService
{
    readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task SubmitOrder(Guid orderId)
    {
        await _publishEndpoint.Publish(new OrderSubmitted
        {
            OrderId = orderId,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

### Sending Commands

Use `ISendEndpointProvider` to send commands to specific endpoints:

```csharp
public class OrderService
{
    readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderService(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task SubmitOrder(Guid orderId)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
            new Uri("queue:submit-order"));
        
        await endpoint.Send(new SubmitOrder
        {
            OrderId = orderId
        });
    }
}
```

### Request/Response

```csharp
// Request
public record GetOrderStatus(Guid OrderId);

// Response
public record OrderStatus(Guid OrderId, string Status);

// Consumer
public class GetOrderStatusConsumer : IConsumer<GetOrderStatus>
{
    public async Task Consume(ConsumeContext<GetOrderStatus> context)
    {
        await context.RespondAsync(new OrderStatus(
            context.Message.OrderId, "Shipped"));
    }
}

// Request Client
public class OrderStatusService
{
    readonly IRequestClient<GetOrderStatus> _client;

    public OrderStatusService(IRequestClient<GetOrderStatus> client)
    {
        _client = client;
    }

    public async Task<OrderStatus> GetStatus(Guid orderId)
    {
        var response = await _client.GetResponse<OrderStatus>(
            new GetOrderStatus(orderId));
        
        return response.Message;
    }
}
```

---

## Advanced Topics

### Sagas

Sagas are long-running workflows that maintain state:

```csharp
// Saga State
public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public DateTime Created { get; set; }
}

// Saga State Machine
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderCompleted> OrderCompleted { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(x => x.Message.OrderId));
        Event(() => OrderCompleted, x => x.CorrelateById(x => x.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context => 
                {
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.Created = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
                .Publish(context => new OrderProcessingStarted(context.Instance.CorrelationId)));

        During(Submitted,
            When(OrderCompleted)
                .TransitionTo(Completed)
                .Finalize());
    }
}
```

### Message Retry

```csharp
// Configure retry in consumer definition
protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
{
    endpointConfigurator.UseMessageRetry(r =>
    {
        r.Interval(3, 1000); // Retry 3 times with 1 second interval
        // Or use exponential backoff
        r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
    });
}
```

### Outbox Pattern

To ensure reliable message publishing:

```csharp
protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
{
    endpointConfigurator.UseInMemoryOutbox();
}
```

### Middleware

```csharp
x.UsingRabbitMq((context, cfg) =>
{
    cfg.UseMessageScope(context);
    cfg.UseConsumeFilter<LoggingFilter>(context);
    
    cfg.ConfigureEndpoints(context);
});
```

---

## Best Practices

1. **Use records** with `public` properties and `{ get; init; }` accessors
2. **Name commands** in verb-noun format (e.g., `SubmitOrder`)
3. **Name events** in noun-verb past tense (e.g., `OrderSubmitted`)
4. **Keep messages simple** - only include state, not behavior
5. **Avoid message inheritance** - it leads to polymorphic routing issues
6. **Use CorrelationId** for message correlation in distributed systems
7. **Use `ConfigureEndpoints`** for automatic endpoint configuration
8. **Use the Outbox pattern** for reliable message publishing with database operations

---

## Additional Resources

- [Official Documentation](https://masstransit.io/documentation)
- [MassTransit GitHub](https://github.com/MassTransit/MassTransit)
- [Discord Channel](https://discord.com/channels/682551963402325523)
- [YouTube Channel](https://www.youtube.com/@PhatBoyG)
