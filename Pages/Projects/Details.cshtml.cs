using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Details;

namespace OnSet.Pages.Projects
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;
        public Details(IMediator mediator) => _mediator = mediator;

        public Model Data { get; private set; }
        public async Task OnGetAsync(Query query) 
        {
            await _mediator.Send(query);
        }
    }
}
