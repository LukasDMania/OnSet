using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnSet.Domain.Enums;

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
                u.OwnsOne(x => x.HomeAddress, ha =>
                {
                    ha.Property(p => p.Street).IsRequired(false);
                    ha.Property(p => p.City).IsRequired(false);
                    ha.Property(p => p.ProvinceOrState).IsRequired(false);
                    ha.Property(p => p.Country).IsRequired(false);
                    ha.Property(p => p.ZipCode).IsRequired(false);
                });
                u.Navigation(x => x.HomeAddress).IsRequired(false);

                u.Property(x => x.SpokenLanguages)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v ?? new List<Languages>(), (JsonSerializerOptions?)null),
                        v => string.IsNullOrWhiteSpace(v) ? new List<Languages>() : JsonSerializer.Deserialize<List<Languages>>(v, (JsonSerializerOptions?)null) ?? new List<Languages>()
                    )
                    .Metadata.SetValueComparer(new ValueComparer<List<Languages>>(
                        (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                        v => (v ?? new()).Aggregate(0, (acc, next) => HashCode.Combine(acc, next.GetHashCode())),
                        v => v == null ? new List<Languages>() : v.ToList()));
            });

            modelBuilder.Entity<Document>(d => {
                d.OwnsOne(x => x.Metadata);

                d.HasOne(d => d.Contract)
                 .WithOne(c => c.Document)
                 .HasForeignKey<Contract>(c => c.DocumentId)
                 .OnDelete(DeleteBehavior.Restrict);

                d.Property(x => x.Tags)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v ?? new List<DocumentTags>(), (JsonSerializerOptions?)null),
                        v => string.IsNullOrWhiteSpace(v) ? new List<DocumentTags>() : JsonSerializer.Deserialize<List<DocumentTags>>(v, (JsonSerializerOptions?)null) ?? new List<DocumentTags>()
                    )
                    .Metadata.SetValueComparer(new ValueComparer<List<DocumentTags>>(
                        (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
                        v => (v ?? new()).Aggregate(0, (acc, next) => HashCode.Combine(acc, next.GetHashCode())),
                        v => v == null ? new List<DocumentTags>() : v.ToList()));
            });

            modelBuilder.Entity<Contract>(c => {
                c.OwnsOne(x => x.Signature);

            });

            modelBuilder.Entity<Project>(p => {
                p.OwnsOne(x => x.Location, loc =>
                {
                    loc.Property(p => p.Street).IsRequired(false);
                    loc.Property(p => p.City).IsRequired(false);
                    loc.Property(p => p.ProvinceOrState).IsRequired(false);
                    loc.Property(p => p.Country).IsRequired(false);
                    loc.Property(p => p.ZipCode).IsRequired(false);
                });
                p.Navigation(x => x.Location).IsRequired(false);
            });

        }
    }
}
