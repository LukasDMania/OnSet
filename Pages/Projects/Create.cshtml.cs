using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Create;

namespace OnSet.Pages.Projects
{
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        public Create(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command CreateProjectCommand { get; set; } = null!;

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(CreateProjectCommand);

            return this.RedirectToPageJson("Index");
        }
    }
}
