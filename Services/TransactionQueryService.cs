using System.Numerics;
using InsightsApi.Abstractions;
using InsightsApi.Infrastructure;
using InsightsApi.Models;
using InsightsApi.Options;
using InsightsApi.Dtos;
using Microsoft.Extensions.Options;

namespace InsightsApi.Services;

public sealed class TransactionQueryService
{
    private readonly ITransactionRepository _repo;
    private readonly IClock _clock;
    private readonly IOptions<InsightsOptions> _opt;

    public TransactionQueryService(ITransactionRepository repo, IClock clock, IOptions<InsightsOptions> opt)
    {
        _repo = repo;
        _clock = clock;
        _opt = opt;
    }

    public async Task<PagedResult<Transaction>> QueryAPageAsync(int days, int page, int pageSize, CancellationToken ct)
    {
        var opt = _opt.Value;
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = opt.DefaultPageSize;

        var to = _clock.UtcNow;
        var from = to.AddDays(-days);

        var all = await _repo.GetAllAsync(ct);
        var filterd = all
        .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
        .OrderByDescending(t => t.CreatedAt)
        .ToList();

        var num = filterd.Count;
        var items = filterd
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

        return new PagedResult<Transaction>
        {
            Page = page,
            PageSize = pageSize,
            Total = num,
            Items = items
        };
    }
}