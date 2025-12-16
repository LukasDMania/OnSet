using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Features.Users.OtherUserDetails;

namespace OnSet.Pages.Users
{
    public class OtherUserDetailsModel : PageModel
    {
        public readonly IMediator _mediator;

        public OtherUserDetailsModel(IMediator mediator)
        {
            _mediator = mediator;           
        }

        public Model? OtherUserDetails { get; private set; }

        public async Task<IActionResult> OnGetAsync(string Id)
        {
            OtherUserDetails = await _mediator.Send(new Query 
            {
                Id = Id
            });

            return Page();
        }
    }
}
