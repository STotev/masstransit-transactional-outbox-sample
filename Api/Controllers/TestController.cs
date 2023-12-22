using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TransactionalOutboxSample.Consumers;
using TransactionalOutboxSample.Consumers.CreateAgent;
using TransactionalOutboxSample.Consumers.CreateCustomer;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IPublishEndpoint _endpoint;

    public TestController(IPublishEndpoint endpoint)
    {
        _endpoint = endpoint;
    }

    [HttpGet("/add-customer")]
    public async Task<IActionResult> AddCustomer()
    {
        await _endpoint.Publish(new CreateCustomerIntegrationCommand
        {
            CustomerName = "Customer Name",
            CorrelationId = NewId.NextGuid()
        });
        
        return Ok();
    }
    
    [HttpGet("/add-agent")]
    public async Task<IActionResult> AddAgent()
    {
        await _endpoint.Publish(new CreateAgentIntegrationCommand()
        {
            AgentName = "Agent Name",
            CorrelationId = NewId.NextGuid()
        });
        
        return Ok();
    }
}