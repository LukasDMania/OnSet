using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Domain.Models;
using OnSet.Features.Projects.Create;

namespace OnSet.Pages.Projects
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;

        public Create(IMediator mediator, UserManager<User> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // UI validation fails (e.g., Required fields missing)  
                return Page();
            }

            var currentUserId = _userManager.GetUserId(User);

            var command = new Command
            (
                Name: Input.Name,
                StartDate: Input.StartDate,
                Status: Input.Status,
                Description: Input.Description,
                ClientName: null,
                ReferenceCode: null,
                Budget: null,
                CreatorRole: Input.CreatorRole,
                CurrentUserId: currentUserId
            );

            var result = await _mediator.Send(command);
            return RedirectToPage("./Details", new { id = result });
            //return this.RedirectToPageJson("Index");
        }
    }
}
