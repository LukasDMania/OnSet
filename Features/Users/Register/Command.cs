using MediatR;
using OnSet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Users.Register
{
    public record Command : IRequest<CommandResult>
    {
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
        public int? YearsExperience { get; init; }
        public string? Bio { get; init; }
        public string? AvatarUrl { get; init; }
        public string? Province { get; init; }
        public string? City { get; init; }
        public string? Street { get; init; }
        public string? Country { get; init; }
        public string? ZipCode { get; init; }

        public List<Languages>? SpokenLanguages { get; init; }
        public bool IsAvailableForBooking { get; init; } = true;
        public DateTime? NextAvailableDate { get; init; }
        public string? EmergencyContactName { get; init; }
        public string? EmergencyContactPhone { get; init; }
    }

    public class CommandResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    }
}
