using MassTransit;

namespace TransactionalOutboxSample.Consumers.CreateCustomer;

public class CreateCustomerConsumerDefinition : ConsumerDefinition<CreateCustomerConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateCustomerConsumer> consumerConfigurator)
    {
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMqConfigurator)
        {
            rabbitMqConfigurator.SetQuorumQueue();
        }
    }
}