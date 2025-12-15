using MediatR;

namespace OnSet.Features.Users.Details
{
    public class Query : IRequest<Model>
    {
        public string? Id { get; init; }
    }
}
