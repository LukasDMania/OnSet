using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Users.Login;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Pages.Users
{
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

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

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

            var result = await _mediator.Send(command);

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
