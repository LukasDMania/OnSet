using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Domain.Models
{
    public class User : IdentityUser
    {
        public FirstName FirstName { get; set; }
        public LastName LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";


        [DisplayFormat(NullDisplayText = "No Main Occupation")]
        public ProjectRoles? MainOccupationRole { get; set; }
        public int? YearsExperience { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public Address HomeAddress { get; set; } = new Address();
        public List<Languages>? SpokenLanguages { get; set; }


        public bool IsAvailableForBooking { get; set; } = true;
        public DateTime? NextAvailableDate { get; set; }

        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? PasswordLastChangedAt { get; set; }

        public string? InternalNotes { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}