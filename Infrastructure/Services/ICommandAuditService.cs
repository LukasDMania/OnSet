namespace OnSet.Infrastructure.Services;

/// <summary>Infrastructure service contract.</summary>

public interface ICommandAuditService
{
    Task RecordAsync(
        string commandName,
        string? correlationId,
        string? userId,
        string? clientIp,
        bool succeeded,
        string? exceptionType,
        string? failureSummary,
        CancellationToken cancellationToken = default);
}
