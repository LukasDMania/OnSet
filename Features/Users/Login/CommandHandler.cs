using MediatR;
using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Models;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Users.Login
{
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly SignInManager<User> _signInManager;

        public CommandHandler(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return Result.Fail("Invalid login attempt.");
            }

            return Result.Ok();
        }
    }
}
