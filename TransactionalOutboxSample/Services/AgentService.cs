using MassTransit;
using TransactionalOutboxSample.Entities;
using TransactionalOutboxSample.IntegrationEvents;

namespace TransactionalOutboxSample.Services;

public interface IAgentService
{
    Task CreateAgent(string agentName);
}

public class AgentService : IAgentService
{
    private readonly IPublishEndpoint _endpoint;
    private readonly AppDbContext _dbContext;

    public AgentService(IPublishEndpoint endpoint, AppDbContext dbContext)
    {
        _endpoint = endpoint;
        _dbContext = dbContext;
    }
    
    public async Task CreateAgent(string agentName)
    {
        var agent = new Agent
        {
            AgentName = agentName
        };

        var newAgent = (await _dbContext.Agents.AddAsync(agent)).Entity;
        
        await _endpoint.Publish(new AgentCreatedIntegrationEvent
        {
            AgentId = newAgent.Id,
            AgentName = newAgent.AgentName
        });
        
        await _dbContext.SaveChangesAsync();
    }
}