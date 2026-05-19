using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.Details
{
    /// <summary>Project detail view model returned by <see cref="Query"/>.</summary>
    public record Model
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ClientName { get; init; }
        public string? ReferenceCode { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }

        public decimal? Budget { get; init; }
        public decimal? ActualCost { get; init; }
        public ProjectStatus Status { get; init; }

        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? ZipCode { get; init; }
        public string? Country { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }

        public string? OwnerName { get; init; }
    }
}
