using MediatR;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.Create
{
    /// <summary>
    /// Creates a new <see cref="Domain.Models.Project"/> owned by the current user.
    /// </summary>
    /// <remarks>POST <c>/Projects/Create</c> via <see cref="Pages.Projects.CreateModel"/>; validated by <see cref="Validator"/>.</remarks>
    public record Command : IRequest<Result>
    {
        [Required]
        [StringLength(100)]
        public string ProjectName { get; init; }

        [StringLength(500)]
        public string? Description { get; init; }

        [StringLength(100)]
        public string? ProductionCompany { get; init; }

        [StringLength(50)]
        public string? ReferenceCode { get; init; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; init; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; init; }

        [Required]
        public ProjectRoles CreatorRole { get; init; }

        // Production company location
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? Country { get; init; }
        public string? ZipCode { get; init; }

        // Invoice details
        public string? InvoiceCompanyName { get; init; }
        public string? InvoiceStreet { get; init; }
        public string? InvoiceCity { get; init; }
        public string? InvoiceProvince { get; init; }
        public string? InvoiceCountry { get; init; }
        public string? InvoiceZipCode { get; init; }
        public string? InvoiceVatNumber { get; init; }
        public string? InvoiceReference { get; init; }
    }
}
