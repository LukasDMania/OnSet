using MediatR;

namespace OnSet.Features.Users.Logout
{
    /// <summary>Signs out the current user.</summary>
    /// <remarks>POST <c>/Users/Logout</c> via <see cref="Pages.Users.LogoutModel"/>.</remarks>
    public record Command : IRequest<Unit>;
}
