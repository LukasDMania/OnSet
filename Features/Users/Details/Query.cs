using MediatR;

namespace OnSet.Features.Users.Details
{
    /// <summary>Loads a user profile by id.</summary>
    /// <remarks>Throws <see cref="Application.Exceptions.NotFoundException"/> when the user does not exist.</remarks>
    public class Query : IRequest<Model>
    {
        public string? Id { get; init; }
    }
}
