using MediatR;

namespace OnSet.Features.Projects.Edit
{
    public record Query : IRequest<Command>
    {
        public int Id { get; init; }
    }
}
