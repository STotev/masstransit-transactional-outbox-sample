using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TransactionalOutboxSample.Entities;

namespace TransactionalOutboxSample;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Agent> Agents { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // MassTransit entities for the Transactional Outbox
        modelBuilder.AddInboxStateEntity(eb => eb.ToTable("__MTInboxState"));
        modelBuilder.AddOutboxMessageEntity(eb => eb.ToTable("__MTOutboxMessage"));
        modelBuilder.AddOutboxStateEntity(eb => eb.ToTable("__MTOutboxState"));

        base.OnModelCreating(modelBuilder);
    }
}