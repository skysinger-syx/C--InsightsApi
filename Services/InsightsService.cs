using InsightsApi.Abstractions;
using InsightsApi.Dtos;
using InsightsApi.Options;
using Microsoft.Extensions.Options;

namespace InsightsApi.Services;

public sealed class InsightsService
{
    private readonly ITransactionRepository _repo;
    private readonly IClock _clock;
    private readonly IOptions<InsightsOptions> _opt;

    public InsightsService(ITransactionRepository repo, IClock clock, IOptions<InsightsOptions> opt)
    {
        _repo = repo;
        _clock = clock;
        _opt = opt;
    }

    public async Task<InsightsSummaryDto> SummaryAsync(int days, CancellationToken ct)
    {
        var opt = _opt.Value;
        var to = _clock.UtcNow;
        var from = to.AddDays(-days);
        var all = await _repo.GetAllAsync(ct);
        var txs = all
        .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
        .ToList();
        var total = txs.Count;
        var gmv = txs.Sum(t => t.Total);
        var avg = total == 0 ? 0m : gmv / total;

        var highValue = txs.Count(t => t.Total >= opt.HighValueThreshold);
        var topAccounts = txs
        .GroupBy(t => t.AccountId)
        .Select(g => new InsightsSummaryDto.TopAccountDto
        {
            AccountId = g.Key,
            Count = g.Count(),
            Spend = g.Sum(x => x.Total)
        })
        .OrderByDescending(x => x.Spend)
        .Take(opt.TopAccounts)
        .ToList();

        var narrative = total == 0
            ? "No transactions found in the selected period."
            : $"In the last {days} days, GMV is {gmv:F2}, AOV is {avg:F2}, " +
              $"{highValue} transactions are above the high-value threshold ({opt.HighValueThreshold:F2}). " +
              (topAccounts.Count > 0 ? $"Top account is {topAccounts[0].AccountId}." : "");

        return new InsightsSummaryDto
        {
            Period = $"last_{days}-days",
            TotalTransactions = total,
            TotalGmv = gmv,
            AvgTransactionValue = avg,
            HighValueTransactions = highValue,
            TopAccounts = topAccounts,
            Narrative = narrative
        };
    }
}
