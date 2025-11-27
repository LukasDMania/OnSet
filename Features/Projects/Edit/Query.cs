using MediatR;

namespace OnSet.Features.Projects.Edit
{
    public class Query : IRequest<Command>
    {
        public int? Id { get; init; }
    }
}
