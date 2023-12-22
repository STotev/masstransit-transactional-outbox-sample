namespace TransactionalOutboxSample.IntegrationEvents;

public record CustomerCreatedIntegrationEvent
{
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
}