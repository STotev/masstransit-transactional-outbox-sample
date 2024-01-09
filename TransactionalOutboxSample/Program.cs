using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TransactionalOutboxSample;
using TransactionalOutboxSample.EfCoreInterceptors;
using TransactionalOutboxSample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TransactionInterceptor>();
builder.Services.AddScoped<IAgentService, AgentService>();

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection(RabbitMqOptions.ConfigurationSection));

var rabbitMqOptions = builder.Services
    .BuildServiceProvider()
    .GetService<IOptions<RabbitMqOptions>>()!.Value;

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));

    var transactionInterceptor = sp.GetRequiredService<TransactionInterceptor>();
    options.AddInterceptors(transactionInterceptor);
});

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();
    
    busRegistrationConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
    
    busRegistrationConfigurator.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox(x =>
        {
            x.DisableDeliveryService();
        });
    });

    // make all consumers use the Transactional Outbox(Inbox)
    busRegistrationConfigurator.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<AppDbContext>(context);
    });

    busRegistrationConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.SetQuorumQueue();
        busFactoryConfigurator.Host(
            rabbitMqOptions.Host,
            rabbitMqOptions.Port,
            rabbitMqOptions.VirtualHost,
            hostConfigurator =>
            {
                hostConfigurator.Username(rabbitMqOptions.Username);
                hostConfigurator.Password(rabbitMqOptions.Password);
            });

        busFactoryConfigurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{
}