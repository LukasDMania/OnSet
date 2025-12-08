using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Users.Logout;

namespace OnSet.Pages.Users
{
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
