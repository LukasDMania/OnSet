using MediatR;

namespace OnSet.Features.Projects.Index
{
    /// <summary>Lists all projects the user belongs to.</summary>
    public class Query : IRequest<Model>
    {
        public string UserId { get; init; } = string.Empty;
    }
}
