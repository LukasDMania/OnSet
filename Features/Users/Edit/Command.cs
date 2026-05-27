using MediatR;
using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Edit
{
    /// <summary>Updates profile fields for a user.</summary>
    /// <remarks>POST <c>/Users/Details</c>; handler enforces self-service only.</remarks>
    public record Command : IRequest<Unit>
    {
        public string UserId { get; set; }

        public AccountType AccountType { get; init; }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Bio { get; init; }
        public string AvatarUrl { get; init; }

        public string MainOccupationRole { get; init; }
        public List<string> SpokenLanguages { get; init; } = [];

        public DateTime? DateOfBirth { get; init; }
        public string PlaceOfBirth { get; init; }
        public string Nationality { get; init; }
        public string NationalRegistrationNumber { get; init; }
        public MaritalStatus? MaritalStatus { get; init; }
        public DietaryPreference? DietaryPreference { get; init; }

        public string Street { get; init; }
        public string City { get; init; }
        public string Province { get; init; }
        public string ZipCode { get; init; }
        public string Country { get; init; }

        public string EmergencyContactName { get; init; }
        public string EmergencyContactPhone { get; init; }
    }
}
