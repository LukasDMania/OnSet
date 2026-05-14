using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using OnSet.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class User : IdentityUser, IAuditableEntity
    {
        [Required]
        public FirstName FirstName { get; set; }

        [Required]
        public LastName LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string? FullName => $"{FirstName} {LastName}";

        [Display(Name = "Main Occupation Role")]
        public ProjectRoles? MainOccupationRole { get; set; }

        [Range(0, 80)]
        [Display(Name = "Years of Experience")]
        public int? YearsExperience { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [Url]
        [Display(Name = "Profile Image")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Home Address")]
        public Address? HomeAddress { get; set; }

        [Display(Name = "Languages Spoken")]
        public List<Languages>? SpokenLanguages { get; set; }

        [Display(Name = "Available for Booking")]
        public bool IsAvailableForBooking { get; set; } = true;

        [DataType(DataType.Date)]
        [Display(Name = "Next Available Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? NextAvailableDate { get; set; }

        [Display(Name = "Emergency Contact")]
        public EmergencyContact? EmergencyContact { get; set; }


        // Meta
        public bool IsActive { get; set; } = true;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastLoginAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PasswordLastChangedAt { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }

        // Navigation
        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();



        public static User Create(
            FirstName firstName,
            LastName lastName,
            ProjectRoles? mainOccupationRole = null,
            int? yearsExperience = null,
            string? bio = null,
            string? avatarUrl = null,
            Address? homeAddress = null,
            List<Languages>? spokenLanguages = null,
            bool isAvailableForBooking = true,
            DateTime? nextAvailableDate = null,
            EmergencyContact? emergencyContact = null
        )
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio;
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;

            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                MainOccupationRole = mainOccupationRole,
                YearsExperience = yearsExperience,
                Bio = bio,
                AvatarUrl = avatarUrl,
                HomeAddress = homeAddress,
                SpokenLanguages = spokenLanguages ?? new List<Languages>(),
                IsAvailableForBooking = isAvailableForBooking,
                NextAvailableDate = nextAvailableDate,
                EmergencyContact = emergencyContact,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        public void UpdateProfile(
            FirstName firstName,
            LastName lastName,
            ProjectRoles? mainOccupationRole,
            int? yearsExperience,
            string? bio,
            string? avatarUrl,
            Address? homeAddress,
            List<Languages>? spokenLanguages,
            bool isAvailableForBooking,
            DateTime? nextAvailableDate,
            EmergencyContact? emergencyContact)
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio;
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;

            if (yearsExperience is < 0 or > 80)
            { 
                throw new DomainRuleException("Years of experience must be between 0 and 80."); 
            }

            if (!isAvailableForBooking)
            {
                nextAvailableDate = null;
            }

            FirstName = firstName;
            LastName = lastName;
            MainOccupationRole = mainOccupationRole;
            YearsExperience = yearsExperience;
            Bio = bio;
            AvatarUrl = avatarUrl;
            HomeAddress = homeAddress;
            SpokenLanguages = spokenLanguages ?? new List<Languages>();
            IsAvailableForBooking = isAvailableForBooking;
            NextAvailableDate = nextAvailableDate;
            EmergencyContact = emergencyContact;
        }
    }
}
