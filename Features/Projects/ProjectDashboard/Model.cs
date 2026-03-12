using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.ProjectDashboard
{
    public record Model
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? ReferenceCode { get; init; }
        public string? ClientName { get; init; }

        public IReadOnlyList<DocumentGroup> DocumentGroups { get; init; } = Array.Empty<DocumentGroup>();
        public bool CanUploadDocuments { get; init; }
    }

    public record DocumentGroup
    {
        public DocumentTags Tag { get; init; }
        public string DisplayName { get; init; } = string.Empty;
        public IReadOnlyList<DocumentDto> Documents { get; init; } = Array.Empty<DocumentDto>();
    }

    public record DocumentDto
    {
        public int Id { get; init; }
        public string FileName { get; init; } = string.Empty;
        public string UploadedBy { get; init; } = string.Empty;
        public DateTime UploadedAt { get; init; }
        public string FilePath { get; init; } = string.Empty;
    }
}

