using MassTransit;
using TransactionalOutboxSample.Services;

namespace TransactionalOutboxSample.Consumers.CreateAgent;

public class CreateAgentConsumer : IConsumer<CreateAgentIntegrationCommand>
{
    private readonly IAgentService _agentService;

    public CreateAgentConsumer(IAgentService agentService)
    {
        _agentService = agentService;
    }
    
    public async Task Consume(ConsumeContext<CreateAgentIntegrationCommand> context)
    {
        await _agentService.CreateAgent(context.Message.AgentName);
    }
}