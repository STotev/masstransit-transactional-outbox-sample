using MassTransit;
using Microsoft.Extensions.Options;
using TransactionalOutboxSample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection(RabbitMqOptions.ConfigurationSection));

var rabbitMqOptions = builder.Services
    .BuildServiceProvider()
    .GetService<IOptions<RabbitMqOptions>>()!.Value;

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();

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