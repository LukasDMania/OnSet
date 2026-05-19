using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OnSet.Utils
{
    /// <summary>
    /// JSON and redirect helpers for Razor Page handlers that return AJAX-friendly responses.
    /// </summary>
    public static class PageModelExtensions
    {
        /// <summary>
        /// Returns JSON containing a local page redirect URL for client-side navigation.
        /// </summary>
        /// <typeparam name="TPage">The page model type.</typeparam>
        /// <param name="controller">The page model instance.</param>
        /// <param name="pageName">Target page name for <see cref="Microsoft.AspNetCore.Mvc.IUrlHelper.Page(string?)"/>.</param>
        /// <returns>A JSON <see cref="ContentResult"/> with a <c>redirect</c> property.</returns>
        public static ActionResult RedirectToPageJson<TPage>(this TPage controller, string pageName)
            where TPage : PageModel =>
            controller.JsonNet(new
            {
                redirect = controller.Url.Page(pageName)
            }
            );

        /// <summary>
        /// Serializes a model to JSON using Newtonsoft.Json settings that ignore reference loops.
        /// </summary>
        /// <param name="controller">The page model instance.</param>
        /// <param name="model">Object to serialize.</param>
        /// <returns>JSON <see cref="ContentResult"/> with <c>application/json</c> content type.</returns>
        public static ContentResult JsonNet(this PageModel controller, object model)
        {
            var serialized = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return new ContentResult
            {
                Content = serialized,
                ContentType = "application/json"
            };
        }
    }
}
