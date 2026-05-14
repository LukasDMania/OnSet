using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnSet.Infrastructure.Results;

namespace OnSet.Infrastructure.Behaviors;

/// <summary>
/// Logs structured latency for each MediatR request. Placed inside command audit but outside validation
/// so elapsed time covers validation + handler, not the audit persistence step.
/// </summary>
public class PerformancePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly Action<ILogger, string, double, bool, string?, Exception?> Completed =
        LoggerMessage.Define<string, double, bool, string?>(
            LogLevel.Information,
            new EventId(10, "MediatrRequestCompleted"),
            "MediatR {RequestName} completed in {ElapsedMs:F2} ms (IsQuery={IsQuery}, CorrelationId={CorrelationId})");

    private static readonly Action<ILogger, string, double, int, Exception?> SlowRequest =
        LoggerMessage.Define<string, double, int>(
            LogLevel.Warning,
            new EventId(11, "MediatrSlowRequest"),
            "Slow MediatR request {RequestName}: {ElapsedMs:F2} ms (threshold {ThresholdMs} ms)");

    private static readonly Action<ILogger, string, double, string?, Exception?> Faulted =
        LoggerMessage.Define<string, double, string?>(
            LogLevel.Warning,
            new EventId(12, "MediatrRequestFaulted"),
            "MediatR {RequestName} faulted after {ElapsedMs:F2} ms (CorrelationId={CorrelationId})");

    private readonly ICurrentUserService _currentUser;
    private readonly ILogger _logger;
    private readonly IOptions<PerformanceTelemetryOptions> _options;

    public PerformancePipelineBehavior(
        ICurrentUserService currentUser,
        ILoggerFactory loggerFactory,
        IOptions<PerformanceTelemetryOptions> options)
    {
        _currentUser = currentUser;
        _logger = loggerFactory.CreateLogger("OnSet.Performance");
        _options = options;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var isQuery = requestType.Name.EndsWith("Query", StringComparison.Ordinal);
        if (isQuery && !_options.Value.LogQueries)
            return await next(cancellationToken);

        var requestName = requestType.FullName ?? requestType.Name;
        var correlationId = _currentUser.CorrelationId;
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);
            sw.Stop();
            var elapsedMs = sw.Elapsed.TotalMilliseconds;

            Completed(_logger, requestName, elapsedMs, isQuery, correlationId, null);

            if (elapsedMs >= _options.Value.SlowRequestThresholdMs)
                SlowRequest(_logger, requestName, elapsedMs, _options.Value.SlowRequestThresholdMs, null);

            if (response is Result r && !r.Success)
            {
                _logger.LogInformation(
                    "MediatR {RequestName} returned unsuccessful Result after {ElapsedMs:F2} ms (IsInvalid={IsInvalid}, ErrorCount={ErrorCount})",
                    requestName,
                    elapsedMs,
                    r.IsInvalid,
                    r.Errors.Count);
            }

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Faulted(_logger, requestName, sw.Elapsed.TotalMilliseconds, correlationId, ex);
            throw;
        }
    }
}
