using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Details
{
    /// <summary>User profile view model returned by <see cref="Query"/>.</summary>
    public record Model
    {
        public string Id { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }

        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        public ProjectRoles? MainOccupationRole { get; init; }
        public int? YearsExperience { get; init; }
        public string? Bio { get; init; }
        public string? AvatarUrl { get; init; }

        // Address VO
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? ZipCode { get; init; }
        public string? Country { get; init; }

        public List<Languages> SpokenLanguages { get; init; } = new List<Languages>();

        public bool IsAvailableForBooking { get; init; }
        public DateTime? NextAvailableDate { get; init; }

        // Emergency contact VO
        public string? EmergencyContactName { get; init; }
        public string? EmergencyContactPhone { get; init; }

        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public string? InternalNotes { get; init; }

        // Nav props
        public List<UserProjectDto> Projects { get; init; } = new();
        public List<ContractDto> Contracts { get; init; } = new();
    }
}