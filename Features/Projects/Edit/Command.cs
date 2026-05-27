using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Authorization;
using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>Updates project metadata; requires <see cref="ProjectPermission.ManageProject"/>.</summary>
    /// <remarks>POST <c>/Projects/Edit/{id}</c>; may throw <see cref="Application.Exceptions.NotFoundException"/> or <see cref="Application.Exceptions.ForbiddenAccessException"/>.</remarks>
    public record Command : IRequest<Unit>, IAuthorizableRequest
    {
        public int Id { get; init; }

        public IEnumerable<IAuthorizationRequirement> GetAuthorizationRequirements() =>
        [
            new ProjectPermissionRequirement(Id, ProjectPermission.ManageProject),
        ];
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ProductionCompany { get; init; }
        public string? ReferenceCode { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }

        // Production company location
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? ZipCode { get; init; }
        public string? Country { get; init; }

        // Invoice details
        public string? InvoiceCompanyName { get; init; }
        public string? InvoiceStreet { get; init; }
        public string? InvoiceCity { get; init; }
        public string? InvoiceProvince { get; init; }
        public string? InvoiceZipCode { get; init; }
        public string? InvoiceCountry { get; init; }
        public string? InvoiceVatNumber { get; init; }
        public string? InvoiceReference { get; init; }

        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
