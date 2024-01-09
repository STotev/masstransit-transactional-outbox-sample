using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using TransactionalOutboxSample;
using TransactionalOutboxSample.Consumers.CreateAgent;
using TransactionalOutboxSample.Consumers.CreateCustomer;
using TransactionalOutboxSample.IntegrationEvents;

namespace Tests;

public class TransactionalOutboxTests
{
    [Fact]
    public async Task Without_Transactional_Outbox()
    {
        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        await using var provider = new ServiceCollection()
            .AddScoped<IConfiguration>(_ => configBuilder.Build())
            .AddDbContext<AppDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                options.UseNpgsql(config.GetConnectionString("Database"));
            })
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<CreateCustomerConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        var correlationId = NewId.NextGuid();
        await harness.Bus.Publish(new CreateCustomerIntegrationCommand()
        {
            CustomerName = "Test",
            CorrelationId = correlationId
        });

        Assert.True(
            await harness.Published.Any<CreateCustomerIntegrationCommand>(f =>
                f.Context.CorrelationId == correlationId));

        var consumerHarness = harness.GetConsumerHarness<CreateCustomerConsumer>();

        Assert.True(
            await consumerHarness.Consumed.Any<CreateCustomerIntegrationCommand>(f =>
                f.Context.CorrelationId == correlationId), "Failed to consume CreateCustomerIntegrationCommand");

        Assert.True(await harness.Published.Any<CustomerCreatedIntegrationEvent>(),
            "Failed to publish CustomerCreatedIntegrationEvent");
    }

    [Fact]
    public async Task With_Transactional_Outbox()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.AddDbContext<AppDbContext>((sp, options) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    options.UseNpgsql(config.GetConnectionString("Database"));
                });

                services.AddMassTransitTestHarness();
            }));

        var harness = application.Services.GetTestHarness();

        await harness.Start();

        var correlationId = NewId.NextGuid();
        await harness.Bus.Publish(new CreateCustomerIntegrationCommand()
        {
            CustomerName = "Test",
            CorrelationId = correlationId
        });

        Assert.True(
            await harness.Published.Any<CreateCustomerIntegrationCommand>(f =>
                f.Context.CorrelationId == correlationId));

        var consumerHarness = harness.GetConsumerHarness<CreateCustomerConsumer>();

        Assert.True(
            await consumerHarness.Consumed.Any<CreateCustomerIntegrationCommand>(f =>
                f.Context.CorrelationId == correlationId), "Failed to consume CreateCustomerIntegrationCommand");

        Assert.True(await harness.Published.Any<CustomerCreatedIntegrationEvent>(),
            "Failed to publish CustomerCreatedIntegrationEvent");
    }
}