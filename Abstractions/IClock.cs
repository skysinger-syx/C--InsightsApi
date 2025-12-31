namespace InsightsApi.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}