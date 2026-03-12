using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Domain.Enums;
using OnSet.Features.Projects.ProjectDashboard;
using OnSet.Features.Projects.UploadDocument;
using OnSet.Infrastructure.Results;

namespace OnSet.Pages.Projects
{
    [Authorize(Policy = "Authenticated")]
    public class ProjectModel : PageModel
    {
        private readonly IMediator _mediator;

        public ProjectModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Model Data { get; private set; } = new();

        [BindProperty]
        public UploadInputModel Upload { get; set; } = new();

        public class UploadInputModel
        {
            [BindProperty]
            public DocumentTags Tag { get; set; }

            [BindProperty]
            public IFormFile? File { get; set; }

            [BindProperty]
            public string? Description { get; set; }
        }

        public async Task OnGetAsync(int id)
        {
            await LoadData(id);
        }

        public async Task<IActionResult> OnPostUploadAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadData(id);
                return Page();
            }

            var command = new Command
            {
                ProjectId = id,
                Tag = Upload.Tag,
                Description = Upload.Description,
                File = Upload.File
            };

            Result result = await _mediator.Send(command);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                await LoadData(id);
                return Page();
            }

            return RedirectToPage("/Projects/Project", new { id });
        }

        private async Task LoadData(int id)
        {
            Data = await _mediator.Send(new Query { Id = id });
        }
    }
}

