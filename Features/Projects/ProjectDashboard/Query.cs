using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Authorization;

namespace OnSet.Features.Projects.ProjectDashboard
{
    /// <summary>Loads the project dashboard (documents, team summary) for a member.</summary>
    /// <remarks>GET <c>/Projects/{Id}</c>; validated by <see cref="Validator"/>.</remarks>
    public class Query : IRequest<Model>, IAuthorizableRequest
    {
        public int? Id { get; init; }

        public IEnumerable<IAuthorizationRequirement> GetAuthorizationRequirements()
        {
            if (Id is null)
            {
                yield break;
            }

            yield return new ProjectPermissionRequirement(Id.Value, ProjectPermission.ViewDashboard);
        }
    }
}
