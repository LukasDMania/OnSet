using MediatR;
using OnSet.Domain.Enums;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.Create
{
    public record Command : IRequest<Result>
    {
        [Required]
        [StringLength(100)]
        public string ProjectName { get; init; }

        [StringLength(500)]
        public string? Description { get; init; }

        [StringLength(100)]
        public string? ClientName { get; init; }

        [StringLength(50)]
        public string? ReferenceCode { get; init; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; init; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; init; }

        [DataType(DataType.Currency)]
        public decimal? Budget { get; init; }

        [Required]
        public ProjectStatus Status { get; init; }

        [Required]
        public ProjectRoles CreatorRole { get; init; }

        // Location (Address VO parts)
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? Country { get; init; }
        public string? ZipCode { get; init; }
    }
}
