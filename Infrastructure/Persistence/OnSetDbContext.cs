using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Models;

namespace OnSet.Infrastructure.Data
{
    public class OnSetDbContext : IdentityDbContext<User>
    {
        public OnSetDbContext(DbContextOptions<OnSetDbContext> options) : base(options) { }

        //Entities
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);     
            
        }
    }
}
