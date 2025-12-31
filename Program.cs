using InsightsApi.Options;
using InsightsApi.Abstractions;
using InsightsApi.Infrastructure;
using InsightsApi.Services;
using InsightsApi.Models;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<TransactionQueryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => Results.Ok(new { status = "ok" }));

app.MapGet("/1-page-transactions", async (
    int days,
    int page,
    int pageSize,
    TransactionQueryService svc,
    ILoggerFactory loggerFactory,
    CancellationToken ct) =>
{
    var log = loggerFactory.CreateLogger("TransactionsEndpoint");
    log.LogInformation("Query transactions. days={Days}, page={Page}, pageSize={PageSize}", days, page, pageSize);
    var result = await svc.QueryAPageAsync(days <= 0 ? 7 : days, page, pageSize, ct);
    return Results.Ok(result);
}
);

app.MapPost("/seed", async (
    int? count,
    ITransactionRepository repo,
    IClock clock,
    ILogger<Program> log,
    CancellationToken ct
) =>
{
    var n = Math.Clamp(count ?? 200, 1, 2000);
    var rng = new Random(12345);
    var txs = new List<Transaction>(n);
    for (int i = 0; i < n; i++)
    {
        var created = clock.UtcNow.AddDays(-rng.NextDouble() * 14); // 最近 14 天
        var accountId = $"A{rng.Next(1, 300):D4}";
        var currency = "USD";

        var lineCount = rng.Next(1, 6);
        var lines = new List<TransactionLine>(lineCount);
        for (int k = 0; k < lineCount; k++)
        {
            var sku = $"SKU-{rng.Next(1, 120):D3}";
            var qty = rng.Next(1, 5);
            var price = Math.Round((decimal)(rng.NextDouble() * 90 + 10), 2);
            lines.Add(new TransactionLine(sku, qty, price));
        }
        txs.Add(new Transaction(Guid.NewGuid(), accountId, created, lines, currency));
    }
    await repo.UpsertBatchAsync(txs, ct);
    var total = await repo.CountAsync(ct);

    log.LogInformation("Seeded {N} transactions. Total now={Total}", n, total);
    return Results.Ok(new { seeded = n, total });
}
);

app.Run();
