namespace OnSet.Application.Authorization;

/// <summary>Resource passed to ASP.NET Core <see cref="Microsoft.AspNetCore.Authorization.IAuthorizationService"/> for project checks.</summary>
public sealed class ProjectResource
{
    public ProjectResource(int projectId)
    {
        ProjectId = projectId;
    }

    public int ProjectId { get; }
}
