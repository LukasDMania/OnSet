using MediatR;

namespace OnSet.Features.Projects.ProjectDashboard
{
    public class Query : IRequest<Model>
    {
        public int? Id { get; init; }
    }
}

