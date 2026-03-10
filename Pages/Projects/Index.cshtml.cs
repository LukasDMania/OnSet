using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Index;
using OnSet.Features.Projects.Join;
using OnSet.Infrastructure.Results;

namespace OnSet.Pages.Projects;

[Authorize(Policy = "Authenticated")]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public Model Data { get; private set; } = new();

    [BindProperty]
    public string JoinCode { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        Data = await _mediator.Send(new Query { UserId = userId });
    }

    public async Task<IActionResult> OnPostJoinAsync(string joinCode)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            // Should not happen due to [Authorize], but keep it defensive.
            return RedirectToPage("/Users/Login");
        }

        var command = new Command
        {
            JoinCode = joinCode,
            UserId = userId
        };

        Result result = await _mediator.Send(command);

        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        // Always reload the list so newly joined projects show up.
        Data = await _mediator.Send(new Query { UserId = userId });
        return Page();
    }
}

