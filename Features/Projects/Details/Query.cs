using MediatR;

namespace OnSet.Features.Projects.Details
{
    public class Query : IRequest<Model>
    {
        public int? Id { get; init; }
    }
}
