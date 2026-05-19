using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Details
{
    /// <summary>Project membership summary on the user profile.</summary>
    public record UserProjectDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public ProjectRoles RoleOnProject { get; init; }
    }
}
