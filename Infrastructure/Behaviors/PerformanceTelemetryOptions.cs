namespace OnSet.Infrastructure.Behaviors;

/// <summary>
/// Optional tuning for <see cref="PerformancePipelineBehavior{TRequest,TResponse}"/> (see appsettings <c>PerformanceTelemetry</c>).
/// </summary>
public class PerformanceTelemetryOptions
{
    public const string SectionName = "PerformanceTelemetry";

    /// <summary>Requests slower than this log at <see cref="LogLevel.Warning"/>.</summary>
    public int SlowRequestThresholdMs { get; set; } = 1000;

    /// <summary>When false, requests whose type name ends with <c>Query</c> are not timed/logged.</summary>
    public bool LogQueries { get; set; } = true;
}
