namespace InsightsApi.Options;

public sealed class InsightsOptions
{
    public decimal HighValueThreshold { get; set; } = 500m;
    public int TopAccounts { get; set; } = 5;
    public int DefaultPageSize { get; set; } = 50;
    public int MaxPageSize { get; set; } = 200;
}