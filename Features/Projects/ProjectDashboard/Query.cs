using MediatR;

namespace OnSet.Features.Projects.ProjectDashboard
{
    /// <summary>Loads the project dashboard (documents, team summary) for a member.</summary>
    /// <remarks>GET <c>/Projects/{Id}</c>; validated by <see cref="Validator"/>.</remarks>
    public class Query : IRequest<Model>
    {
        public int? Id { get; init; }
    }
}

