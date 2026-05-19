namespace OnSet.Application.Authorization;

/// <summary>UI capability flags for the project dashboard (derived from <see cref="ProjectPermission"/>).</summary>
public sealed class ProjectDashboardCapabilities
{
    public bool CanUploadDocuments { get; init; }

    public bool CanManageProject { get; init; }

    public bool CanManageMembers { get; init; }

    public bool CanDeleteProject { get; init; }

    public bool CanDeleteDocuments { get; init; }
}
