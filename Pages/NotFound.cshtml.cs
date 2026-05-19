using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnSet.Pages;

/// <summary>HTTP 404 page when <see cref="Application.Exceptions.NotFoundException"/> is thrown.</summary>
[AllowAnonymous]
public class NotFoundModel : PageModel
{
    public void OnGet()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
    }
}
