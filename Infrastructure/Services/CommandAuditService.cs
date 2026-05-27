using OnSet.Infrastructure.Persistence;

namespace OnSet.Infrastructure.Services;

/// <summary>Infrastructure component.</summary>

public class CommandAuditService : ICommandAuditService
{
    private readonly OnSetDbContext _db;

    public CommandAuditService(OnSetDbContext db) => _db = db;

    public async Task RecordAsync(
        string commandName,
        string? correlationId,
        string? userId,
        string? clientIp,
        bool succeeded,
        string? exceptionType,
        string? failureSummary,
        CancellationToken cancellationToken = default)
    {
        _db.CommandAuditLogs.Add(new CommandAuditLog
        {
            CommandName = commandName,
            CorrelationId = correlationId,
            UserId = userId,
            ClientIp = clientIp,
            OccurredAt = DateTime.UtcNow,
            Succeeded = succeeded,
            ExceptionType = exceptionType,
            FailureSummary = failureSummary
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
