using Microsoft.EntityFrameworkCore;
using OnSet.Application.Authorization;
using OnSet.Application.Exceptions;
using OnSet.Application.Services;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Authorization;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Infrastructure.Services;

/// <summary>Scoped permission evaluator (one EF context per HTTP request / MediatR scope).</summary>
public sealed class ProjectPermissionService : IProjectPermissionService
{
    private readonly OnSetDbContext _context;

    public ProjectPermissionService(OnSetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(
        int projectId,
        string? userId,
        ProjectPermission permission,
        CancellationToken cancellationToken = default)
    {
        var tier = await ResolveTierAsync(projectId, userId, cancellationToken);
        return tier is not null && ProjectPermissionMatrix.Allows(tier.Value, permission);
    }

    /// <inheritdoc />
    public async Task EnsurePermissionAsync(
        int projectId,
        string? userId,
        ProjectPermission permission,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await _context.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId, cancellationToken);

        if (!projectExists)
        {
            throw new NotFoundException("Project", projectId);
        }

        if (!await HasPermissionAsync(projectId, userId, permission, cancellationToken))
        {
            throw new ForbiddenAccessException(
                "You do not have permission to perform this action on the project.");
        }
    }

    /// <inheritdoc />
    public async Task<ProjectDashboardCapabilities> GetDashboardCapabilitiesAsync(
        int projectId,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var tier = await ResolveTierAsync(projectId, userId, cancellationToken);
        if (tier is null)
        {
            return new ProjectDashboardCapabilities();
        }

        return new ProjectDashboardCapabilities
        {
            CanUploadDocuments = ProjectPermissionMatrix.Allows(tier.Value, ProjectPermission.UploadDocuments),
            CanManageProject = ProjectPermissionMatrix.Allows(tier.Value, ProjectPermission.ManageProject),
            CanManageMembers = ProjectPermissionMatrix.Allows(tier.Value, ProjectPermission.ManageMembers),
            CanDeleteProject = ProjectPermissionMatrix.Allows(tier.Value, ProjectPermission.DeleteProject),
            CanDeleteDocuments = ProjectPermissionMatrix.Allows(tier.Value, ProjectPermission.DeleteDocuments),
        };
    }

    private async Task<ProjectAccessTier?> ResolveTierAsync(
        int projectId,
        string? userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var access = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new
            {
                p.OwnerId,
                RoleOnProject = p.UserProjects
                    .Where(up => up.UserId == userId)
                    .Select(up => (ProjectRoles?)up.RoleOnProject)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (access is null)
        {
            return null;
        }

        if (string.Equals(access.OwnerId, userId, StringComparison.Ordinal))
        {
            return ProjectAccessTier.Owner;
        }

        if (access.RoleOnProject is null)
        {
            return null;
        }

        return access.RoleOnProject == ProjectRoles.PRODUCTION
            ? ProjectAccessTier.Manager
            : ProjectAccessTier.Viewer;
    }
}
