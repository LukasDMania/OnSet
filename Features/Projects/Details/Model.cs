using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.Details
{
    /// <summary>Project detail view model returned by <see cref="Query"/>.</summary>
    public record Model
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ProductionCompany { get; init; }
        public string? ReferenceCode { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }

        public string? Street { get; init; }
        public string? City { get; init; }
        public string? Province { get; init; }
        public string? ZipCode { get; init; }
        public string? Country { get; init; }

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

        public string? OwnerName { get; init; }
    }
}
