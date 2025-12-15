using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Details
{
    public record UserProjectDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public ProjectRoles RoleOnProject { get; init; }
    }
}
