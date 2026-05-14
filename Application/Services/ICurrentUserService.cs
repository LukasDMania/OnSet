namespace OnSet
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
        List<string> Roles { get; }
        string? ClientIp { get; }
        /// <summary>Request correlation: X-Correlation-Id header when present, otherwise the ASP.NET request trace identifier.</summary>
        string? CorrelationId { get; }
    }
}

