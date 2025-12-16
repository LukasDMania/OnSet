using MediatR;

namespace OnSet.Features.Users.OtherUserDetails
{
    public class Query : IRequest<Model>
    {
        public string Id { get; set; }
    }
}
