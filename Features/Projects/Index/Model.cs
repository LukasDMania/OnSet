namespace OnSet.Features.Projects.Index
{
    /// <summary>Project list returned by <see cref="Query"/>.</summary>
    public record Model
    {
        public IReadOnlyList<ProjectListItem> Projects { get; init; } = Array.Empty<ProjectListItem>();
    }

    public record ProjectListItem
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? ReferenceCode { get; init; }
        public string? ClientName { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime? EndDate { get; init; }
    }
}