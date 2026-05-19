using OnSet.Domain.Enums;

namespace OnSet.Features.Users.Details
{
    /// <summary>Contract row shown on the user profile.</summary>
    public record ContractDto
    {
        public int Id { get; init; }
        public string? DocumentPath { get; init; }
        public string? DocumentDescription { get; init; }
        public ContractStatus Status { get; init; }
        public DateTime? SignedAt { get; init; }
        public string? SignedByUserId { get; init; }
    }
}
