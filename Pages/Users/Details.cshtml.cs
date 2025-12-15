using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;
using OnSet.Features.Users.Details;
using OnSet.Features.Users.Register;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public DetailsModel(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        public Model? UserDetails { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_currentUser.IsAuthenticated)
            {
                return Challenge();
            }

            string userIdToFetch = _currentUser.UserId;

            UserDetails = await _mediator.Send(new Query
            {
                Id = userIdToFetch
            });

            if (UserDetails == null)
            {
                return NotFound();
            }

            return Page();
        }
    }

}
