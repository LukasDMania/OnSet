using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Authorization;
using OnSet.Application.Services;
using System.Security.Claims;

namespace OnSet.Infrastructure.Authorization;

/// <summary>
/// ASP.NET Core authorization handler (scoped). Delegates to scoped <see cref="IProjectPermissionService"/>.
/// </summary>
public sealed class ProjectPermissionHandler : AuthorizationHandler<ProjectPermissionRequirement, ProjectResource>
{
    private readonly IProjectPermissionService _permissionService;

    public ProjectPermissionHandler(IProjectPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectPermissionRequirement requirement,
        ProjectResource resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var projectId = resource.ProjectId;

        if (await _permissionService.HasPermissionAsync(projectId, userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
