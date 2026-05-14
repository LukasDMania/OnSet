using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnSet.Pages;

[AllowAnonymous]
public class NotFoundModel : PageModel
{
    public void OnGet()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
    }
}
