using Microsoft.EntityFrameworkCore;
using OnSet.Infrastructure.Data;

namespace OnSet.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app) 
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using OnSetDbContext context = scope.ServiceProvider.GetRequiredService<OnSetDbContext>();

            context.Database.Migrate();
        }
    }
}
