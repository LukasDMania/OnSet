using MediatR;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Users.Register
{
    /// <summary>
    /// Registers a new <see cref="Domain.Models.User"/> and signs them in.
    /// </summary>
    /// <remarks>POST <c>/Users/Register</c>; validated by <see cref="Validator"/>.</remarks>
    public record Command : IRequest<Result>
    {
        [Required]
        public AccountType AccountType { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; init; }

        // Domain fields
        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }

        public ProjectRoles? MainOccupationRole { get; init; }
        public string? Bio { get; init; }
        public string? AvatarUrl { get; init; }
        public string? Province { get; init; }
        public string? City { get; init; }
        public string? Street { get; init; }
        public string? Country { get; init; }
        public string? ZipCode { get; init; }

        public List<Languages>? SpokenLanguages { get; init; }

        public DateTime? DateOfBirth { get; init; }
        public string? PlaceOfBirth { get; init; }
        public string? Nationality { get; init; }
        public string? NationalRegistrationNumber { get; init; }
        public MaritalStatus? MaritalStatus { get; init; }
        public DietaryPreference? DietaryPreference { get; init; }

        public string? EmergencyContactName { get; init; }
        public string? EmergencyContactPhone { get; init; }
    }
}
