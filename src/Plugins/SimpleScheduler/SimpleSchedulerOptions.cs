namespace SimpleScheduler;

public record SimpleSchedulerOptions
{
    internal const string BatchSizePath = "simpleScheduler";

    public int BatchSize { get; set; } = 25;
}
