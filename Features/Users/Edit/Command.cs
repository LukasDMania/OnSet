using MediatR;
using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Edit
{
    public record Command : IRequest<Unit>
    {
        public string UserId { get; set; }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Bio { get; init; }
        public string AvatarUrl { get; init; }

        public string MainOccupationRole { get; init; }
        public int? YearsExperience { get; init; }
        public List<string> SpokenLanguages { get; init; } = [];
        public bool IsAvailableForBooking { get; init; }
        public DateTime? NextAvailableDate { get; init; }

        public string Street { get; init; }
        public string City { get; init; }
        public string Province { get; init; }
        public string ZipCode { get; init; }
        public string Country { get; init; }

        public string EmergencyContactName { get; init; }
        public string EmergencyContactPhone { get; init; }
    }
}
