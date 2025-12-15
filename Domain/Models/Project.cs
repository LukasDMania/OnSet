using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    public class Project : IOnSetEntity
    {
        private Project() { }

        [Key]
        [Column("ProjectID")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Client Name")]
        public string? ClientName { get; set; }

        [StringLength(50)]
        [Display(Name = "Reference Code")]
        public string? ReferenceCode { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal? Budget { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Actual Cost")]
        public decimal? ActualCost { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        public Address? Location { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Owner")]
        public string? OwnerId { get; set; }
        public virtual User? Owner { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();


        private void AddCreator(string creatorId, ProjectRoles role)
        {
            var userProject = UserProject.Create(creatorId, role);
            this.UserProjects.Add(userProject);
        }

        public static Project Create(
        string name,
        DateTime startDate,
        ProjectStatus status,
        ProjectRoles creatorRole,
        string ownerId,
        string? description = null,
        string? clientName = null,
        string? referenceCode = null,
        decimal? budget = null,
        Address? location = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            if (startDate == default)
                throw new ArgumentException("Start date is required.", nameof(startDate));

            if (budget < 0)
                throw new ArgumentOutOfRangeException(nameof(budget), "Budget cannot be negative.");

            var project = new Project
            {
                Name = name.Trim(),
                Description = description,
                ClientName = clientName,
                ReferenceCode = referenceCode,
                StartDate = startDate,
                Budget = budget,
                Status = status,
                OwnerId = ownerId,
                Location = location,
                CreatedAt = DateTime.UtcNow
            };

            project.AddCreator(ownerId, creatorRole);

            return project;
        }
    }
}
