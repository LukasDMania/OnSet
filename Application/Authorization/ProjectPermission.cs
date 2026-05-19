namespace OnSet.Application.Authorization;

/// <summary>Project-scoped actions enforced by <see cref="ProjectPermissionRequirement"/> and <see cref="Services.IProjectPermissionService"/>.</summary>
public enum ProjectPermission
{
    ViewDashboard,
    ManageProject,
    UploadDocuments,
    ManageMembers,
    DeleteProject,
    DeleteDocuments,
    ArchiveDocuments,
    EditSchedule,
}
