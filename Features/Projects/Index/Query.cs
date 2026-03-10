using MediatR;

namespace OnSet.Features.Projects.Index
{
    public class Query : IRequest<Model>
    {
        public string UserId { get; init; } = string.Empty;
    }
}
