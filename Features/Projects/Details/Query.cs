using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Authorization;

namespace OnSet.Features.Projects.Details
{
    /// <summary>Loads full project details for users with manage permission.</summary>
    /// <remarks>GET <c>/Projects/Details/{Id}</c>; owner or production (<see cref="ProjectAccessTier.Manager"/>).</remarks>
    public class Query : IRequest<Model>, IAuthorizableRequest
    {
        public int? Id { get; init; }

        public IEnumerable<IAuthorizationRequirement> GetAuthorizationRequirements()
        {
            if (Id is null)
            {
                yield break;
            }

            yield return new ProjectPermissionRequirement(Id.Value, ProjectPermission.ManageProject);
        }
    }
}
