using MediatR;

namespace OnSet.Features.Projects.Create
{
    public record Command(string Name, string Description) : IRequest<int>;
}
