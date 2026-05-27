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
    /// <summary>Domain model or value object.</summary>
    public class User : IdentityUser, IAuditableEntity
    {
        [Required]
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; } = AccountType.CREW;

        [Required]
        public FirstName FirstName { get; set; }

        [Required]
        public LastName LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string? FullName => $"{FirstName} {LastName}";

        [Display(Name = "Main Occupation Role")]
        public ProjectRoles? MainOccupationRole { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [Url]
        [Display(Name = "Profile Image")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Home Address")]
        public Address? HomeAddress { get; set; }

        [Display(Name = "Languages Spoken")]
        public List<Languages>? SpokenLanguages { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200)]
        [Display(Name = "Place of Birth")]
        public string? PlaceOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Nationality")]
        public string? Nationality { get; set; }

        [StringLength(50)]
        [Display(Name = "National Registration Number")]
        public string? NationalRegistrationNumber { get; set; }

        [Display(Name = "Marital Status")]
        public MaritalStatus? MaritalStatus { get; set; }

        [Display(Name = "Dietary Preference")]
        public DietaryPreference? DietaryPreference { get; set; }

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
            AccountType accountType,
            FirstName firstName,
            LastName lastName,
            ProjectRoles? mainOccupationRole = null,
            string? bio = null,
            string? avatarUrl = null,
            Address? homeAddress = null,
            List<Languages>? spokenLanguages = null,
            DateTime? dateOfBirth = null,
            string? placeOfBirth = null,
            string? nationality = null,
            string? nationalRegistrationNumber = null,
            MaritalStatus? maritalStatus = null,
            DietaryPreference? dietaryPreference = null,
            EmergencyContact? emergencyContact = null
        )
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio;
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;
            placeOfBirth = string.IsNullOrWhiteSpace(placeOfBirth) ? null : placeOfBirth;
            nationality = string.IsNullOrWhiteSpace(nationality) ? null : nationality;
            nationalRegistrationNumber = string.IsNullOrWhiteSpace(nationalRegistrationNumber) ? null : nationalRegistrationNumber;

            return new User
            {
                AccountType = accountType,
                FirstName = firstName,
                LastName = lastName,
                MainOccupationRole = mainOccupationRole,
                Bio = bio,
                AvatarUrl = avatarUrl,
                HomeAddress = homeAddress,
                SpokenLanguages = spokenLanguages ?? new List<Languages>(),
                DateOfBirth = dateOfBirth,
                PlaceOfBirth = placeOfBirth,
                Nationality = nationality,
                NationalRegistrationNumber = nationalRegistrationNumber,
                MaritalStatus = maritalStatus,
                DietaryPreference = dietaryPreference,
                EmergencyContact = emergencyContact,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }

        public static User Create(
            FirstName firstName,
            LastName lastName,
            ProjectRoles? mainOccupationRole = null,
            string? bio = null,
            string? avatarUrl = null,
            Address? homeAddress = null,
            List<Languages>? spokenLanguages = null,
            DateTime? dateOfBirth = null,
            string? placeOfBirth = null,
            string? nationality = null,
            string? nationalRegistrationNumber = null,
            MaritalStatus? maritalStatus = null,
            DietaryPreference? dietaryPreference = null,
            EmergencyContact? emergencyContact = null
        )
        {
            return Create(
                AccountType.CREW,
                firstName,
                lastName,
                mainOccupationRole,
                bio,
                avatarUrl,
                homeAddress,
                spokenLanguages,
                dateOfBirth,
                placeOfBirth,
                nationality,
                nationalRegistrationNumber,
                maritalStatus,
                dietaryPreference,
                emergencyContact
            );
        }

        public void UpdateProfile(
            AccountType accountType,
            FirstName firstName,
            LastName lastName,
            ProjectRoles? mainOccupationRole,
            string? bio,
            string? avatarUrl,
            Address? homeAddress,
            List<Languages>? spokenLanguages,
            DateTime? dateOfBirth,
            string? placeOfBirth,
            string? nationality,
            string? nationalRegistrationNumber,
            MaritalStatus? maritalStatus,
            DietaryPreference? dietaryPreference,
            EmergencyContact? emergencyContact)
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio;
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;
            placeOfBirth = string.IsNullOrWhiteSpace(placeOfBirth) ? null : placeOfBirth;
            nationality = string.IsNullOrWhiteSpace(nationality) ? null : nationality;
            nationalRegistrationNumber = string.IsNullOrWhiteSpace(nationalRegistrationNumber) ? null : nationalRegistrationNumber;

            AccountType = accountType;
            FirstName = firstName;
            LastName = lastName;
            MainOccupationRole = mainOccupationRole;
            Bio = bio;
            AvatarUrl = avatarUrl;
            HomeAddress = homeAddress;
            SpokenLanguages = spokenLanguages ?? new List<Languages>();
            DateOfBirth = dateOfBirth;
            PlaceOfBirth = placeOfBirth;
            Nationality = nationality;
            NationalRegistrationNumber = nationalRegistrationNumber;
            MaritalStatus = maritalStatus;
            DietaryPreference = dietaryPreference;
            EmergencyContact = emergencyContact;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
