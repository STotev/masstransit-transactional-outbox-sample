using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TransactionalOutboxSample.EfCoreInterceptors;

public class TransactionInterceptor : DbTransactionInterceptor
{
    private readonly ILogger<TransactionInterceptor> _logger;

    public TransactionInterceptor(ILogger<TransactionInterceptor> logger)
    {
        _logger = logger;
    }

    public override DbTransaction TransactionStarted(DbConnection connection, TransactionEndEventData eventData, DbTransaction result)
    {
        System.Diagnostics.Debug.WriteLine($"Transaction Started -> ContextId: {eventData.Context?.ContextId} TransactionId: {eventData.TransactionId}");

        _logger.LogDebug("----- Transaction Started -> ContextId: {ContextId} TransactionId: {TransactionId}",
            eventData.Context?.ContextId,
            eventData.TransactionId);

        return base.TransactionStarted(connection, eventData, result);
    }

    public override ValueTask<DbTransaction> TransactionStartedAsync(DbConnection connection, TransactionEndEventData eventData, DbTransaction result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        System.Diagnostics.Debug.WriteLine($"Transaction Started Async -> ContextId: {eventData.Context?.ContextId} TransactionId: {eventData.TransactionId}");

        _logger.LogDebug("----- Transaction Started -> ContextId: {ContextId} TransactionId: {TransactionId}",
            eventData.Context?.ContextId,
            eventData.TransactionId);

        return base.TransactionStartedAsync(connection, eventData, result, cancellationToken);
    }

    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        System.Diagnostics.Debug.WriteLine($"Transaction Committed -> ContextId: {eventData.Context?.ContextId} TransactionId: {eventData.TransactionId}");

        _logger.LogDebug("----- Transaction Committed -> ContextId: {ContextId} TransactionId: {TransactionId}",
            eventData.Context?.ContextId,
            eventData.TransactionId);

        base.TransactionCommitted(transaction, eventData);
    }

    public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
        System.Diagnostics.Debug.WriteLine($"Transaction Committed Async -> ContextId: {eventData.Context?.ContextId} TransactionId: {eventData.TransactionId}");

        _logger.LogDebug("----- Transaction Committed -> ContextId: {ContextId} TransactionId: {TransactionId}",
            eventData.Context?.ContextId,
            eventData.TransactionId);

        return base.TransactionCommittedAsync(transaction, eventData, cancellationToken);
    }
}
