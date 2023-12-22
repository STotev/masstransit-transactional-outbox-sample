using MassTransit;

namespace TransactionalOutboxSample.Consumers.CreateAgent;

public class CreateAgentConsumerDefinition : ConsumerDefinition<CreateAgentConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateAgentConsumer> consumerConfigurator)
    {
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMqConfigurator)
        {
            rabbitMqConfigurator.SetQuorumQueue();
        }
    }
}