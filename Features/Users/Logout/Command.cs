using MediatR;

namespace OnSet.Features.Users.Logout
{
    public record Command : IRequest<Unit>;
}
