using OnSet.Application.Authorization;
using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.ProjectDashboard
{
    /// <summary>Project dashboard view model returned by <see cref="Query"/>.</summary>
    public record Model
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? ReferenceCode { get; init; }
        public string? ProductionCompany { get; init; }

        public IReadOnlyList<DocumentGroup> DocumentGroups { get; init; } = Array.Empty<DocumentGroup>();

        public ProjectDashboardCapabilities Capabilities { get; init; } = new();
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
