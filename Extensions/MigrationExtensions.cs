using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Extensions
{
    /// <summary>Development-time EF Core migration helpers.</summary>
    public static class MigrationExtensions
    {
        /// <summary>Applies pending EF Core migrations on startup (Development only).</summary>
        /// <param name="app">The application builder.</param>
        public static void ApplyMigrations(this IApplicationBuilder app) 
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using OnSetDbContext context = scope.ServiceProvider.GetRequiredService<OnSetDbContext>();

            context.Database.Migrate();
        }
    }
}
