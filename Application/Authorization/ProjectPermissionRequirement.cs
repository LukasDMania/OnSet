using Microsoft.AspNetCore.Authorization;

namespace OnSet.Application.Authorization;

/// <summary>Requires a specific <see cref="ProjectPermission"/> on a project.</summary>
public sealed class ProjectPermissionRequirement : IAuthorizationRequirement
{
    public ProjectPermissionRequirement(int projectId, ProjectPermission permission)
    {
        ProjectId = projectId;
        Permission = permission;
    }

    public int ProjectId { get; }

    public ProjectPermission Permission { get; }
}
