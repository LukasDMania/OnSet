using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Infrastructure.Filters;

namespace OnSet.Pages;

/// <summary>HTTP 400 page when <see cref="Application.Exceptions.DomainRuleException"/> is thrown.</summary>
[AllowAnonymous]
public class BadRequestModel : PageModel
{
    public string? Details { get; private set; }

    public void OnGet()
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        Details = TempData[DomainExceptionFilter.DomainRuleMessageKey] as string;
    }
}
