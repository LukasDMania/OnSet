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


            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            modelBuilder.Entity<User>(u =>
            {
                u.OwnsOne(x => x.FirstName, fn =>
                {
                    fn.Property(p => p.Value).HasColumnName("FirstName");
                });

                u.OwnsOne(x => x.LastName, ln =>
                {
                    ln.Property(p => p.Value).HasColumnName("LastName");
                });

                u.OwnsOne(x => x.EmergencyContact);
                u.OwnsOne(x => x.HomeAddress);
            });

            modelBuilder.Entity<Document>(d => {
                d.OwnsOne(x => x.Metadata);

                d.HasOne(d => d.Contract)
                 .WithOne(c => c.Document)
                 .HasForeignKey<Contract>(c => c.DocumentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Contract>(c => {
                c.OwnsOne(x => x.Signature);

            });

            modelBuilder.Entity<Project>(p => {
                p.OwnsOne(x => x.Location);
            });

        }
    }
}
