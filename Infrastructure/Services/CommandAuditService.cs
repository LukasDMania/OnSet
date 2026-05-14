using OnSet.Infrastructure.Data;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Infrastructure.Services;

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
        _db.Set<CommandAuditLog>().Add(new CommandAuditLog
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
