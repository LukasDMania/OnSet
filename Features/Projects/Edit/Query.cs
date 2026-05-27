using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Authorization;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>Loads project data for the edit form.</summary>
    public record Query : IRequest<Command>, IAuthorizableRequest
    {
        public int Id { get; init; }

        public IEnumerable<IAuthorizationRequirement> GetAuthorizationRequirements() =>
        [
            new ProjectPermissionRequirement(Id, ProjectPermission.ManageProject),
        ];
    }
}
