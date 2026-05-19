using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Users.Login;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Pages.Users
{
    /// <summary>Sign-in page: GET shows the form; POST runs <see cref="Command"/>.</summary>
    /// <remarks>Route <c>/Users/Login</c>. See OpenAPI tag <c>Users</c>.</remarks>
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IMediator _mediator;

        public LoginModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        /// <summary>Razor Page handler for this route (see OpenAPI *Razor Pages* section).</summary>

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }

        /// <param name="returnUrl">Optional local URL to redirect to after successful sign-in.</param>
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        /// <param name="returnUrl">Optional local URL after sign-in.</param>
        /// <returns><see cref="Page"/> on validation failure; otherwise redirect.</returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var command = new Command
            {
                Email = Input.Email,
                Password = Input.Password,
                RememberMe = Input.RememberMe
            };

            Result result = await _mediator.Send(command);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                return Page();
            }

            return LocalRedirect(ReturnUrl);
        }
    }
}
