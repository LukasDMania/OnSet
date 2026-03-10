using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Users.Logout;

namespace OnSet.Pages.Users
{
    [Authorize(Policy = "Authenticated")]
    public class LogoutModel : PageModel
    {
        private readonly IMediator _mediator;

        public LogoutModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            await _mediator.Send(new Command());

            if (returnUrl != null)
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Index");
        }
    }
}
