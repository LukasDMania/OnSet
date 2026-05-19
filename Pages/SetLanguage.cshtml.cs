using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnSet.Pages;

/// <summary>Sets the ASP.NET Core localization cookie and redirects.</summary>
/// <remarks>Route <c>/culture/set</c>; supported cultures: en, nl, fr.</remarks>
[AllowAnonymous]
public class SetLanguageModel : PageModel
{
    public IActionResult OnGet(string culture, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return RedirectToPage("/Index");
        }

        try
        {
            CultureInfo.GetCultureInfo(culture);
        }
        catch (CultureNotFoundException)
        {
            return RedirectToPage("/Index");
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Lax
            });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Index");
    }
}
