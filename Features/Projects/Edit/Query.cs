using MediatR;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>Loads project data for the edit form.</summary>
    public record Query : IRequest<Command>
    {
        public int Id { get; init; }
    }
}
