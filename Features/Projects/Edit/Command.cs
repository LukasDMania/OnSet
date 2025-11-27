using MediatR;

namespace OnSet.Features.Projects.Edit
{
    public class Command : IRequest<Unit>
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
    }
}
