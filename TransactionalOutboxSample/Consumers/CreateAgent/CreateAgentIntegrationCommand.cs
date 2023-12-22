using MassTransit;

namespace TransactionalOutboxSample.Consumers.CreateAgent;

public class CreateAgentIntegrationCommand : CorrelatedBy<Guid>
{
    public string AgentName { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
}