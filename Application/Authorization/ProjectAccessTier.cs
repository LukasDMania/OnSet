namespace OnSet.Application.Authorization;

/// <summary>Resolved access tier for a user on a project (mapped from owner id and <see cref="Domain.Enums.ProjectRoles"/>).</summary>
public enum ProjectAccessTier
{
    Owner,
    Manager,
    Viewer,
}
