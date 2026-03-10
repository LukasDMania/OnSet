using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Index;

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

    public async Task OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        Data = await _mediator.Send(new Query { UserId = userId });
    }
}

