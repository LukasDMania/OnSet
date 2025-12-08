using MediatR;
using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Models;

namespace OnSet.Features.Users.Login
{
    public class CommandHandler : IRequestHandler<Command, CommandResult>
    {
        private readonly SignInManager<User> _signInManager;

        public CommandHandler(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return new CommandResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid login attempt." }
                };
            }

            return new CommandResult { Success = true };
        }
    }
}
