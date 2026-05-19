using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Domain.Enums;
using OnSet.Features.Projects.Create;
using OnSet.Infrastructure.Results;
using OnSet.Utils;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Pages.Projects
{
    /// <summary>Razor Page handler (documented in OpenAPI under Razor Pages).</summary>
    [Authorize(Policy = "Authenticated")]
        public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>Razor Page handler for this route (see OpenAPI *Razor Pages* section).</summary>

        public class InputModel
        {
            [Required]
            public string ProjectName { get; set; }

            public string? Description { get; set; }
            public string? ClientName { get; set; }
            public string? ReferenceCode { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [DataType(DataType.Date)]
            public DateTime? EndDate { get; set; }

            public decimal? Budget { get; set; }

            [Required]
            public ProjectStatus Status { get; set; }

            [Required]
            public ProjectRoles CreatorRole { get; set; }

            public string? Street { get; set; }
            public string? City { get; set; }
            public string? Province { get; set; }
            public string? Country { get; set; }
            public string? ZipCode { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var command = new Command
            {
                ProjectName = Input.ProjectName,
                Description = Input.Description,
                ClientName = Input.ClientName,
                ReferenceCode = Input.ReferenceCode,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate,
                Budget = Input.Budget,
                Status = Input.Status,
                CreatorRole = Input.CreatorRole,
                Street = Input.Street,
                City = Input.City,
                Province = Input.Province,
                Country = Input.Country,
                ZipCode = Input.ZipCode
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

            return RedirectToPage(PageRoutes.ProjectsIndex);
        }
    }
}
