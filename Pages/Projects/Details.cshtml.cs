using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Projects.Details;

namespace OnSet.Pages.Projects
{
    /// <summary>Razor Page handler (documented in OpenAPI under Razor Pages).</summary>
    [Authorize(Policy = "Authenticated")]
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;

        public DetailsModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Model Data { get; private set; } = null!;

        /// <param name="query">Bound route/query with project <c>Id</c>.</param>
        public async Task OnGetAsync(Query query)
        {
            Data = await _mediator.Send(query);
        }
    }
}
