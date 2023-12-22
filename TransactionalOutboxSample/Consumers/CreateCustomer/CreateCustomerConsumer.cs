using MassTransit;
using TransactionalOutboxSample.Entities;
using TransactionalOutboxSample.IntegrationEvents;

namespace TransactionalOutboxSample.Consumers.CreateCustomer;

public class CreateCustomerConsumer : IConsumer<CreateCustomerIntegrationCommand>
{
    private readonly AppDbContext _dbContext;

    public CreateCustomerConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<CreateCustomerIntegrationCommand> context)
    {
        var customer = new Customer
        {
            CustomerName = context.Message.CustomerName
        };

        var newCustomer = (await _dbContext.Customers.AddAsync(customer)).Entity;
        
        await _dbContext.SaveChangesAsync();

        await context.Publish(new CustomerCreatedIntegrationEvent
        {
            CustomerId = newCustomer.Id,
            CustomerName = newCustomer.CustomerName
        });
    }
}