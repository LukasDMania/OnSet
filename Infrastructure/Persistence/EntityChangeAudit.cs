namespace OnSet.Infrastructure.Persistence;

/// <summary>Infrastructure component.</summary>

public class EntityChangeAudit
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? ChangedByUserId { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime ChangedAt { get; set; }
}
