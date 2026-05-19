using OnSet.Application.Authorization;

namespace OnSet.Application.Services;

/// <summary>
/// Project permission matrix (scoped; uses EF Core). Shared by MediatR authorization and ASP.NET Core <see cref="Microsoft.AspNetCore.Authorization.IAuthorizationHandler"/> implementations.
/// </summary>
public interface IProjectPermissionService
{
    Task<bool> HasPermissionAsync(
        int projectId,
        string? userId,
        ProjectPermission permission,
        CancellationToken cancellationToken = default);

    Task EnsurePermissionAsync(
        int projectId,
        string? userId,
        ProjectPermission permission,
        CancellationToken cancellationToken = default);

    Task<ProjectDashboardCapabilities> GetDashboardCapabilitiesAsync(
        int projectId,
        string? userId,
        CancellationToken cancellationToken = default);
}
