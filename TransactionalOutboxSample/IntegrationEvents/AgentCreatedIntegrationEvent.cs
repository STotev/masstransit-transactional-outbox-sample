namespace TransactionalOutboxSample.IntegrationEvents;

public record AgentCreatedIntegrationEvent
{
    public long AgentId { get; set; }
    public string AgentName { get; set; } = default!;
}