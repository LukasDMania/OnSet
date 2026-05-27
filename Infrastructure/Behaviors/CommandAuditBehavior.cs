using MediatR;
using Microsoft.Extensions.Logging;
using OnSet.Infrastructure.Results;
using OnSet.Infrastructure.Services;
using OnSet.Application.Services;

namespace OnSet.Infrastructure.Behaviors;

/// <summary>
/// Persists one row per MediatR command (skips types whose name ends with <c>Query</c>).
/// Runs as an outer pipeline step so validation failures are still recorded.
/// Audit persistence failures never change the command outcome or replace thrown exceptions.
/// </summary>
public class CommandAuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Action<ILogger, string, Exception?> AuditWriteFailed =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "CommandAuditWriteFailed"),
            "Command audit write failed for {CommandName}; command outcome unchanged.");

    private readonly ICommandAuditService _commandAudit;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger _logger;

    public CommandAuditBehavior(
        ICommandAuditService commandAudit,
        ICurrentUserService currentUser,
        ILoggerFactory loggerFactory)
    {
        _commandAudit = commandAudit;
        _currentUser = currentUser;
        _logger = loggerFactory.CreateLogger("OnSet.CommandAudit");
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        if (requestType.Name.EndsWith("Query", StringComparison.Ordinal))
            return await next(cancellationToken);

        var commandName = requestType.FullName ?? requestType.Name;
        var userId = string.IsNullOrWhiteSpace(_currentUser.UserId) ? null : _currentUser.UserId;
        var clientIp = _currentUser.ClientIp;
        var correlationId = _currentUser.CorrelationId;

        try
        {
            var response = await next(cancellationToken);
            var (ok, summary) = DescribeOutcome(response);
            await TryRecordAsync(
                commandName,
                correlationId,
                userId,
                clientIp,
                ok,
                exceptionType: null,
                summary,
                cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            await TryRecordAsync(
                commandName,
                correlationId,
                userId,
                clientIp,
                succeeded: false,
                ex.GetType().FullName ?? ex.GetType().Name,
                ex.Message,
                cancellationToken);
            throw;
        }
    }

    private async Task TryRecordAsync(
        string commandName,
        string? correlationId,
        string? userId,
        string? clientIp,
        bool succeeded,
        string? exceptionType,
        string? failureSummary,
        CancellationToken cancellationToken)
    {
        try
        {
            await _commandAudit.RecordAsync(
                commandName,
                correlationId,
                userId,
                clientIp,
                succeeded,
                exceptionType,
                failureSummary,
                cancellationToken);
        }
        catch (Exception ex)
        {
            AuditWriteFailed(_logger, commandName, ex);
        }
    }

    private static (bool Ok, string? Summary) DescribeOutcome(TResponse response)
    {
        if (response is Result r)
        {
            if (r.Success)
                return (true, null);
            var text = r.Errors.Count > 0 ? string.Join("; ", r.Errors) : "Command failed";
            return (false, text);
        }

        return (true, null);
    }
}
