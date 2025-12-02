using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Models;

namespace OnSet.Infrastructure.Data
{
    public class OnSetDbContext : IdentityDbContext<User>
    {
        public OnSetDbContext(DbContextOptions<OnSetDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);     
            
        }
    }
}
