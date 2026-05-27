namespace OnSet.Application.Services;

/// <summary>
/// Provides information about the authenticated user and the current HTTP request for handlers and auditing.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Identity user id of the signed-in user; empty when anonymous.</summary>
    string UserId { get; }

    /// <summary>Display user name from claims.</summary>
    string UserName { get; }

    /// <summary>True when <see cref="UserId"/> is present.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Role names assigned to the current user.</summary>
    List<string> Roles { get; }

    /// <summary>Client IP address when available.</summary>
    string? ClientIp { get; }

    /// <summary>Request correlation: X-Correlation-Id header when present, otherwise the ASP.NET request trace identifier.</summary>
    string? CorrelationId { get; }
}
