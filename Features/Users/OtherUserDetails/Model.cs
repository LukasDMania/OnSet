using OnSet.Domain.Enums;

namespace OnSet.Features.Users.OtherUserDetails
{
    /// <summary>Public profile view model for another user.</summary>
    public record Model
    {
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }

        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        public ProjectRoles? MainOccupationRole { get; init; }
        public string? Bio { get; init; }
        public string? AvatarUrl { get; init; }

        public List<Languages> SpokenLanguages { get; init; } = new List<Languages>();

        public DateTime? LastLoginAt { get; init; }
    }
}
