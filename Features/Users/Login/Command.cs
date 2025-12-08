using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Features.Users.Login
{
    public record Command : IRequest<CommandResult>
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

    public class CommandResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    }
}
