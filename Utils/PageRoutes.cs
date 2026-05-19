namespace OnSet.Utils
{
    /// <summary>
    /// Central Razor Page route constants to avoid magic strings in redirects.
    /// </summary>
    public static class PageRoutes
    {
        /// <summary>Route to the project list page (<c>/Projects</c>).</summary>
        public const string ProjectsIndex = "/Projects/Index";

        /// <summary>Route to project details (<c>/Projects/Details</c>).</summary>
        public const string ProjectsDetails = "/Projects/Details";
    }
}
