using InsightsApi.Abstractions;
using InsightsApi.Models;
using System.Collections.Concurrent;

namespace InsightsApi.Infrastructure;

public sealed class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly ConcurrentDictionary<Guid, Transaction> _store = new();

    public Task UpsertBatchAsync(IEnumerable<Transaction> txs, CancellationToken ct)
    {
        foreach (var tx in txs)
        {
            ct.ThrowIfCancellationRequested();
            _store[tx.Id] = tx;
        }
        return Task.CompletedTask;
    }

    public Task<List<Transaction>> GetAllAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_store.Values.ToList());
    }

    public Task<int> CountAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_store.Count);
    }
}