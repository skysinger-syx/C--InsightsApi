namespace InsightsApi.Models;

public sealed record TransactionLine(string Sku, int Qty, decimal UnitPrice)
{
    public decimal LineTotal => Qty * UnitPrice;
}