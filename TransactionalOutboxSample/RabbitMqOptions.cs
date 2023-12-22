using System.ComponentModel.DataAnnotations;

namespace TransactionalOutboxSample;

/// <summary>
/// RabbitMq settings used in MassTransit
/// </summary>
public class RabbitMqOptions
{
    public const string ConfigurationSection = "RabbitMq";

    public string Host { get; set; } = null!;
    public ushort Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}