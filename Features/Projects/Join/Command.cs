using MediatR;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.Join
{
    /// <summary>
    /// Adds the current user to an existing project using a join code.
    /// </summary>
    /// <remarks>POST <c>/Projects?handler=Join</c> via <see cref="Pages.Projects.IndexModel.OnPostJoinAsync"/>.</remarks>
    public record Command : IRequest<Result>
    {
        [Required]
        [StringLength(50)]
        public string JoinCode { get; init; } = string.Empty;

        [Required]
        public string UserId { get; init; } = string.Empty;
    }
}
