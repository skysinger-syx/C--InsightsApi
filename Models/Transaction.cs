namespace InsightsApi.Models;

public sealed record Transaction(
    Guid Id,
    string AccountId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<TransactionLine> Lines,
    string Currency
)
{
    public decimal Total => Lines.Sum(l => l.LineTotal);
}