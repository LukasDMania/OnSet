using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Application.Services;
using OnSet.Domain.Enums;
using OnSet.Features.Projects.Join;
using OnSet.Infrastructure.Authorization;
using OnSet.Infrastructure.Results;

namespace OnSet.Pages
{
    /// <summary>Razor Page handler for this route (see OpenAPI *Razor Pages* section).</summary>
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public IndexModel(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [BindProperty]
        public string JoinCode { get; set; } = string.Empty;

        public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

        public bool IsProductionAccount =>
            User.HasClaim(AccountTypeClaimsTransformation.ClaimType, AccountType.PRODUCTION.ToString());

        public bool IsCrewAccount => IsAuthenticated && !IsProductionAccount;

        public async Task OnGetAsync()
        {
            if (!IsAuthenticated)
            {
                ViewData["UseLandingLayout"] = true;
            }

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostJoinAsync(string? joinCode)
        {
            if (!IsAuthenticated)
            {
                return RedirectToPage("/Users/Login", new { returnUrl = Url.Page("/Index") });
            }

            var userId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToPage("/Users/Login", new { returnUrl = Url.Page("/Index") });
            }

            var command = new Command
            {
                JoinCode = joinCode ?? JoinCode,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return Page();
            }

            TempData["JoinSuccess"] = true;
            return RedirectToPage();
        }
    }
}
