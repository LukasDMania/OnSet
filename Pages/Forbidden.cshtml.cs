using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnSet.Pages;

/// <summary>HTTP 403 page when <see cref="Application.Exceptions.ForbiddenAccessException"/> is thrown.</summary>
[AllowAnonymous]
public class ForbiddenModel : PageModel
{
    public void OnGet()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}
