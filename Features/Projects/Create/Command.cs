using MediatR;
using OnSet.Domain.Enums;

namespace OnSet.Features.Projects.Create
{
    public record Command(
        string Name,
        DateTime StartDate,
        ProjectStatus Status,
        string? Description,
        string? ClientName,
        string? ReferenceCode,
        decimal? Budget,
        ProjectRoles CreatorRole,
        string CurrentUserId

    ) : IRequest<int>;
}
