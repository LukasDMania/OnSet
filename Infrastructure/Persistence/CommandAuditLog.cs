namespace OnSet.Infrastructure.Persistence;

public class CommandAuditLog
{
    public long Id { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? ClientIp { get; set; }
    public DateTime OccurredAt { get; set; }
    public bool Succeeded { get; set; }
    /// <summary>Full CLR name when failure was an exception; null for logical failures (e.g. Result.Fail).</summary>
    public string? ExceptionType { get; set; }
    public string? FailureSummary { get; set; }
}
