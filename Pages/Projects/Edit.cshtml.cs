using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Edit;
using OnSet.Utils;

namespace OnSet.Pages.Projects
{
    /// <summary>Razor Page handler (documented in OpenAPI under Razor Pages).</summary>
    [Authorize(Policy = "Authenticated")]
        public class EditModel : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public Command Data { get; set; }

        public EditModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(Query query)
        {
            Data = await _mediator.Send(query);
        }

        public async Task<ActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _mediator.Send(Data);

            return RedirectToPage("./Details", new { id = Data.Id });
        }
    }
}
