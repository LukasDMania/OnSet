using MediatR;

namespace OnSet.Features.Projects.Details
{
    /// <summary>Loads project details for a member of the project.</summary>
    /// <remarks>GET <c>/Projects/Details/{Id}</c>; throws <see cref="Application.Exceptions.ForbiddenAccessException"/> when not a member.</remarks>
    public class Query : IRequest<Model>
    {
        public int? Id { get; init; }
    }
}
