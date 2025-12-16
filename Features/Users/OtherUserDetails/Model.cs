using OnSet.Features.Users.Details;

namespace OnSet.Features.Users.OtherUserDetails
{
    public record Model
    {
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }

        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        public string? MainOccupationRole { get; init; }
        public int? YearsExperience { get; init; }
        public string? Bio { get; init; }
        public string? AvatarUrl { get; init; }

        public List<string> SpokenLanguages { get; init; } = new List<string>();

        public bool IsAvailableForBooking { get; init; }
        public DateTime? NextAvailableDate { get; init; }

        public DateTime? LastLoginAt { get; init; }
    }
}
