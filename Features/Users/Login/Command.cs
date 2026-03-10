using MediatR;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Users.Login
{
    public record Command : IRequest<Result>
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; init; } = false;
    }
}
