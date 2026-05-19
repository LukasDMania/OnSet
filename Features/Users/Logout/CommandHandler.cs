using MediatR;
using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Models;

namespace OnSet.Features.Users.Logout
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly SignInManager<User> _signInManager;

        public CommandHandler(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return Unit.Value;
        }
    }
}
