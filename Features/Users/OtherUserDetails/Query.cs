using MediatR;

namespace OnSet.Features.Users.OtherUserDetails
{
    /// <summary>Loads another user's public profile by id.</summary>
    public class Query : IRequest<Model>
    {
        public string Id { get; set; }
    }
}
