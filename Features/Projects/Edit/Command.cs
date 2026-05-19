using MediatR;
using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>Updates project metadata; only the project owner may edit.</summary>
    /// <remarks>POST <c>/Projects/Edit/{id}</c>; may throw <see cref="Application.Exceptions.NotFoundException"/> or <see cref="Application.Exceptions.ForbiddenAccessException"/>.</remarks>
    public record Command : IRequest<Unit>
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
    }
}
