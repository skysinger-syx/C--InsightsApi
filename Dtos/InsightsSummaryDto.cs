namespace InsightsApi.Dtos;

public sealed class InsightsSummaryDto
{
    public required string Period { get; init; }
    public required int TotalTransactions { get; init; }
    public required decimal TotalGmv { get; init; }
    public required decimal AvgTransactionValue { get; init; }
    public required int HighValueTransactions { get; init; }

    public required List<TopAccountDto> TopAccounts { get; init; }
    public required string Narrative { get; init; }

    public sealed class TopAccountDto
    {
        public required string AccountId { get; init; }
        public required int Count { get; init; }
        public required decimal Spend { get; init; }
    }


}