using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnSet.Domain.Models
{
    /// <summary>Domain model or value object.</summary>
    public class Project : IOnSetEntity, IAuditableEntity
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
        [Display(Name = "Production Company")]
        public string? ProductionCompany { get; set; }

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

        public Address? ProductionCompanyLocation { get; set; }

        [StringLength(200)]
        public string? InvoiceCompanyName { get; set; }

        public Address? InvoiceAddress { get; set; }

        [StringLength(50)]
        public string? InvoiceVatNumber { get; set; }

        [StringLength(100)]
        public string? InvoiceReference { get; set; }

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
            var userProject = UserProject.Create(creatorId, role, this);
            this.UserProjects.Add(userProject);
        }

        public static Project Create(
        string name,
        DateTime startDate,
        ProjectRoles creatorRole,
        string ownerId,
        string? description = null,
        string? productionCompany = null,
        string? referenceCode = null,
        Address? productionCompanyLocation = null,
        string? invoiceCompanyName = null,
        Address? invoiceAddress = null,
        string? invoiceVatNumber = null,
        string? invoiceReference = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.", nameof(name));

            if (startDate == default)
                throw new ArgumentException("Start date is required.", nameof(startDate));

            var project = new Project
            {
                Name = name.Trim(),
                Description = description,
                ProductionCompany = productionCompany,
                ReferenceCode = referenceCode,
                StartDate = startDate,
                OwnerId = ownerId,
                ProductionCompanyLocation = productionCompanyLocation,
                InvoiceCompanyName = invoiceCompanyName,
                InvoiceAddress = invoiceAddress,
                InvoiceVatNumber = invoiceVatNumber,
                InvoiceReference = invoiceReference,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            project.AddCreator(ownerId, creatorRole);

            return project;
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
            return Create(
                name: name,
                startDate: startDate,
                creatorRole: creatorRole,
                ownerId: ownerId,
                description: description,
                productionCompany: clientName,
                referenceCode: referenceCode,
                productionCompanyLocation: location
            );
        }

        public void UpdateDetails(
        string name,
        DateTime startDate,
        string? description = null,
        string? productionCompany = null,
        string? referenceCode = null,
        DateTime? endDate = null,
        Address? productionCompanyLocation = null,
        string? invoiceCompanyName = null,
        Address? invoiceAddress = null,
        string? invoiceVatNumber = null,
        string? invoiceReference = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name is required.");

            if (startDate == default)
                throw new ArgumentException("Start date is required.");

            if (endDate.HasValue && endDate.Value < startDate)
                throw new ArgumentException("End date cannot be before the start date.");

            Name = name.Trim();
            Description = string.IsNullOrWhiteSpace(description) ? null : description;
            ProductionCompany = string.IsNullOrWhiteSpace(productionCompany) ? null : productionCompany;
            ReferenceCode = string.IsNullOrWhiteSpace(referenceCode) ? null : referenceCode;

            StartDate = startDate;
            EndDate = endDate;
            ProductionCompanyLocation = productionCompanyLocation;
            InvoiceCompanyName = string.IsNullOrWhiteSpace(invoiceCompanyName) ? null : invoiceCompanyName;
            InvoiceAddress = invoiceAddress;
            InvoiceVatNumber = string.IsNullOrWhiteSpace(invoiceVatNumber) ? null : invoiceVatNumber;
            InvoiceReference = string.IsNullOrWhiteSpace(invoiceReference) ? null : invoiceReference;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(
        string name,
        DateTime startDate,
        ProjectStatus status,
        string? description = null,
        string? clientName = null,
        string? referenceCode = null,
        DateTime? endDate = null,
        decimal? budget = null,
        Address? location = null)
        {
            UpdateDetails(
                name: name,
                startDate: startDate,
                description: description,
                productionCompany: clientName,
                referenceCode: referenceCode,
                endDate: endDate,
                productionCompanyLocation: location
            );
        }
    }
}
