using InsightsApi.Models;

namespace InsightsApi.Abstractions;

public interface ITransactionRepository
{
    Task UpsertBatchAsync(IEnumerable<Transaction> txs, CancellationToken ct);
    Task<List<Transaction>> GetAllAsync(CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
}