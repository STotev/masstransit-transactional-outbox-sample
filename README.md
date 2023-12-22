## Setup and running steps

1. Create empty database
2. Update `appsettings.json` with your credentials
3. Run `dotnet ef database update -p TransactionalOutboxSample`
4. Run `Api` and `TransactionalOutboxSample` projects
5. From Swagger
   * Send `/add-customer` - the consumer in this case publishes another message by immediately using the `ConsumerContext`
   * Send `/add-agent` - the consumer in this case invokes the `AgentService` that uses the `IPublishEndpoint` to publish a message 

## Problem
MassTransit is configured as Transactional Outbox and disables the delivery service.

In both cases, the messages are delivered right after the consumer completes.

## Expectation
I would expect the consumer to complete without sending the messages and leave it to the delivery service.
