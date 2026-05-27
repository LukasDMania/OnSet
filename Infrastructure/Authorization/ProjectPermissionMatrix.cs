using OnSet.Application.Authorization;

namespace OnSet.Infrastructure.Authorization;

/// <summary>Maps <see cref="ProjectAccessTier"/> to allowed <see cref="ProjectPermission"/> values.</summary>
internal static class ProjectPermissionMatrix
{
    private static readonly IReadOnlyDictionary<ProjectAccessTier, HashSet<ProjectPermission>> Permissions =
        new Dictionary<ProjectAccessTier, HashSet<ProjectPermission>>
        {
            [ProjectAccessTier.Owner] =
            [
                ProjectPermission.ViewDashboard,
                ProjectPermission.ManageProject,
                ProjectPermission.UploadDocuments,
                ProjectPermission.ManageMembers,
                ProjectPermission.DeleteProject,
                ProjectPermission.DeleteDocuments,
                ProjectPermission.ArchiveDocuments,
                ProjectPermission.EditSchedule,
            ],
            [ProjectAccessTier.Manager] =
            [
                ProjectPermission.ViewDashboard,
                ProjectPermission.ManageProject,
                ProjectPermission.UploadDocuments,
                ProjectPermission.ManageMembers,
                ProjectPermission.DeleteDocuments,
                ProjectPermission.ArchiveDocuments,
                ProjectPermission.EditSchedule,
            ],
            [ProjectAccessTier.Viewer] =
            [
                ProjectPermission.ViewDashboard,
            ],
        };

    public static bool Allows(ProjectAccessTier tier, ProjectPermission permission)
    {
        return Permissions.TryGetValue(tier, out var set) && set.Contains(permission);
    }
}
