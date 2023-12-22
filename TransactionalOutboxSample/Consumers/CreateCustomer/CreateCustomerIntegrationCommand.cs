using MassTransit;

namespace TransactionalOutboxSample.Consumers.CreateCustomer;

public class CreateCustomerIntegrationCommand : CorrelatedBy<Guid>
{
    public string CustomerName { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
}