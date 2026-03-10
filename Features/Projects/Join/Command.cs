using MediatR;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Projects.Join
{
    public record Command : IRequest<Result>
    {
        [Required]
        [StringLength(50)]
        public string JoinCode { get; init; } = string.Empty;

        [Required]
        public string UserId { get; init; } = string.Empty;
    }
}
