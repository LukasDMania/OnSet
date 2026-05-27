using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Utils
{
    /// <summary>
    /// Dev-only seed data. Wipe DB and call SeedAsync() on startup in Development.
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<OnSetDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            await db.Database.MigrateAsync();

            // ---------------------------------------------------------------
            // WIPE ? children before parents to respect FK constraints
            // ---------------------------------------------------------------

            db.Contracts.RemoveRange(db.Contracts);
            db.Documents.RemoveRange(db.Documents);
            db.UserProjects.RemoveRange(db.UserProjects);
            db.Projects.RemoveRange(db.Projects);
            db.Users.RemoveRange(db.Users);

            await db.SaveChangesAsync();

            var (admin, director, cameraOp, soundOp, actor1, actor2, production) =
                await SeedUsersAsync(userManager);

            var (project1, project2, project3) =
                await SeedProjectsAsync(db, admin, director, production);

            await SeedMembershipsAsync(db, project1, project2, project3,
                director, cameraOp, soundOp, actor1, actor2, production, admin);

            var (doc1, doc2, doc3) =
                await SeedDocumentsAsync(db, project1, project2, admin, director);

            await SeedContractsAsync(db, doc1, doc2, doc3, actor1, actor2, cameraOp);
        }

        // ---------------------------------------------------------------
        // USERS
        // ---------------------------------------------------------------

        private static async Task<(User admin, User director, User cameraOp, User soundOp,
            User actor1, User actor2, User production)> SeedUsersAsync(UserManager<User> userManager)
        {
            var admin = await CreateUserAsync(userManager,
                email: "admin@onset.dev",
                password: "Admin1234!",
                firstName: "Admin",
                lastName: "User",
                role: ProjectRoles.PRODUCTION,
                bio: "Platform administrator.",
                languages: new List<Languages> { Languages.ENGLISH, Languages.DUTCH }
            );

            var director = await CreateUserAsync(userManager,
                email: "director@onset.dev",
                password: "Dev1234!",
                firstName: "Claire",
                lastName: "Dubois",
                role: ProjectRoles.DIRECTOR,
                bio: "Experienced film director.",
                languages: new List<Languages> { Languages.FRENCH, Languages.ENGLISH }
            );

            var cameraOp = await CreateUserAsync(userManager,
                email: "camera@onset.dev",
                password: "Dev1234!",
                firstName: "Lars",
                lastName: "Vermeersch",
                role: ProjectRoles.CAMERA,
                bio: "Camera operator specialising in documentary.",
                languages: new List<Languages> { Languages.DUTCH, Languages.ENGLISH }
            );

            var soundOp = await CreateUserAsync(userManager,
                email: "sound@onset.dev",
                password: "Dev1234!",
                firstName: "Sophie",
                lastName: "Maes",
                role: ProjectRoles.SOUND,
                bio: "Location sound recordist.",
                languages: new List<Languages> { Languages.DUTCH, Languages.FRENCH }
            );

            var actor1 = await CreateUserAsync(userManager,
                email: "actor1@onset.dev",
                password: "Dev1234!",
                firstName: "James",
                lastName: "Cole",
                role: ProjectRoles.ACTOR,
                bio: "Theatre and film actor.",
                languages: new List<Languages> { Languages.ENGLISH }
            );

            var actor2 = await CreateUserAsync(userManager,
                email: "actor2@onset.dev",
                password: "Dev1234!",
                firstName: "Nina",
                lastName: "Peeters",
                role: ProjectRoles.ACTOR,
                bio: "Emerging actress.",
                languages: new List<Languages> { Languages.DUTCH, Languages.ENGLISH }
            );

            var production = await CreateUserAsync(userManager,
                email: "production@onset.dev",
                password: "Dev1234!",
                firstName: "Tomas",
                lastName: "Janssen",
                role: ProjectRoles.PRODUCTION,
                bio: "Line producer with commercial and short film experience.",
                languages: new List<Languages> { Languages.DUTCH, Languages.ENGLISH, Languages.FRENCH }
            );

            return (admin, director, cameraOp, soundOp, actor1, actor2, production);
        }

        // ---------------------------------------------------------------
        // PROJECTS
        // ---------------------------------------------------------------

        private static async Task<(Project project1, Project project2, Project project3)> SeedProjectsAsync(
            OnSetDbContext db,
            User admin,
            User director,
            User production)
        {
            var project1 = Project.Create(
                name: "The Last Harvest",
                startDate: new DateTime(2025, 3, 1),
                creatorRole: ProjectRoles.PRODUCTION,
                ownerId: admin.Id,
                description: "A short drama set in rural Flanders.",
                productionCompany: "Flanders Film Fund",
                referenceCode: "DEV-001",
                productionCompanyLocation: new Address(
                    street: "Kortrijksesteenweg 1",
                    city: "Ghent",
                    provinceOrState: "East Flanders",
                    country: "Belgium",
                    zipCode: "9000"
                )
            );

            var project2 = Project.Create(
                name: "Urban Echoes",
                startDate: new DateTime(2025, 6, 15),
                creatorRole: ProjectRoles.DIRECTOR,
                ownerId: director.Id,
                description: "A documentary exploring urban regeneration in Brussels.",
                productionCompany: "RTBF",
                referenceCode: "DEV-002",
                productionCompanyLocation: new Address(
                    street: "Rue de la Loi 16",
                    city: "Brussels",
                    provinceOrState: "Brussels Capital",
                    country: "Belgium",
                    zipCode: "1000"
                )
            );

            var project3 = Project.Create(
                name: "Still Waters",
                startDate: new DateTime(2024, 11, 1),
                creatorRole: ProjectRoles.PRODUCTION,
                ownerId: production.Id,
                description: "A completed music video for an indie artist.",
                productionCompany: "Independent",
                referenceCode: "DEV-003"
            );

            await db.Projects.AddRangeAsync(project1, project2, project3);
            await db.SaveChangesAsync();

            return (project1, project2, project3);
        }

        // ---------------------------------------------------------------
        // MEMBERSHIPS
        // ---------------------------------------------------------------

        private static async Task SeedMembershipsAsync(
            OnSetDbContext db,
            Project project1,
            Project project2,
            Project project3,
            User director,
            User cameraOp,
            User soundOp,
            User actor1,
            User actor2,
            User production,
            User admin)
        {
            var memberships = new List<UserProject>
            {
                // project1 ? The Last Harvest (admin already added as owner via Project.Create)
                UserProject.Create(director.Id,   ProjectRoles.DIRECTOR,   project1.Id),
                UserProject.Create(cameraOp.Id,   ProjectRoles.CAMERA,     project1.Id),
                UserProject.Create(soundOp.Id,    ProjectRoles.SOUND,      project1.Id),
                UserProject.Create(actor1.Id,     ProjectRoles.ACTOR,      project1.Id),
                UserProject.Create(actor2.Id,     ProjectRoles.ACTOR,      project1.Id),

                // project2 ? Urban Echoes (director already added as owner via Project.Create)
                UserProject.Create(cameraOp.Id,   ProjectRoles.CAMERA,     project2.Id),
                UserProject.Create(soundOp.Id,    ProjectRoles.SOUND,      project2.Id),
                UserProject.Create(production.Id, ProjectRoles.PRODUCTION, project2.Id),

                // project3 ? Still Waters (production already added as owner via Project.Create)
                UserProject.Create(admin.Id,      ProjectRoles.PRODUCTION, project3.Id),
                UserProject.Create(actor1.Id,     ProjectRoles.ACTOR,      project3.Id),
            };

            await db.UserProjects.AddRangeAsync(memberships);
            await db.SaveChangesAsync();
        }

        // ---------------------------------------------------------------
        // DOCUMENTS
        // ---------------------------------------------------------------

        private static async Task<(Document doc1, Document doc2, Document doc3)> SeedDocumentsAsync(
            OnSetDbContext db,
            Project project1,
            Project project2,
            User admin,
            User director)
        {
            var doc1 = new Document(
                projectId: project1.Id,
                userId: admin.Id,
                metadata: new FileMetadata(
                    fileName: "callsheet_day1.pdf",
                    sizeBytes: 102400,
                    mimeType: "application/pdf"
                ),
                filePath: "/dev/uploads/project1/callsheet_day1.pdf",
                description: "Call sheet for shoot day 1."
            );

            var doc2 = new Document(
                projectId: project1.Id,
                userId: director.Id,
                metadata: new FileMetadata(
                    fileName: "scenario_v2.pdf",
                    sizeBytes: 204800,
                    mimeType: "application/pdf"
                ),
                filePath: "/dev/uploads/project1/scenario_v2.pdf",
                description: "Final shooting script revision 2."
            );

            var doc3 = new Document(
                projectId: project2.Id,
                userId: director.Id,
                metadata: new FileMetadata(
                    fileName: "shoot_schedule.pdf",
                    sizeBytes: 51200,
                    mimeType: "application/pdf"
                ),
                filePath: "/dev/uploads/project2/shoot_schedule.pdf",
                description: "Full shoot schedule for Urban Echoes."
            );

            await db.Documents.AddRangeAsync(doc1, doc2, doc3);
            await db.SaveChangesAsync();

            return (doc1, doc2, doc3);
        }

        // ---------------------------------------------------------------
        // CONTRACTS
        // ---------------------------------------------------------------

        private static async Task SeedContractsAsync(
            OnSetDbContext db,
            Document doc1,
            Document doc2,
            Document doc3,
            User actor1,
            User actor2,
            User cameraOp)
        {
            // Pending ? no action taken yet
            var contract1 = new Contract(
                documentId: doc1.Id,
                userId: actor1.Id
            );

            // Signed
            var contract2 = new Contract(
                documentId: doc2.Id,
                userId: actor2.Id
            );
            contract2.Sign(actor2.Id);

            // Declined
            var contract3 = new Contract(
                documentId: doc3.Id,
                userId: cameraOp.Id
            );
            contract3.Decline("Scheduling conflict, unavailable on those dates.");

            await db.Contracts.AddRangeAsync(contract1, contract2, contract3);
            await db.SaveChangesAsync();
        }

        // ---------------------------------------------------------------
        // HELPER ? CREATE SINGLE USER
        // ---------------------------------------------------------------

        private static async Task<User> CreateUserAsync(
            UserManager<User> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            ProjectRoles? role,
            string bio,
            List<Languages> languages)
        {
            var existing = await userManager.FindByEmailAsync(email);

            if (existing != null)
            {
                return existing;
            }

            var accountType = role == ProjectRoles.PRODUCTION
                ? AccountType.PRODUCTION
                : AccountType.CREW;

            var user = User.Create(
                accountType: accountType,
                firstName: new FirstName(firstName),
                lastName: new LastName(lastName),
                mainOccupationRole: role,
                bio: bio,
                spokenLanguages: languages
            );

            user.UserName = email;
            user.Email = email;
            user.EmailConfirmed = true;

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed user '{email}': {errors}");
            }

            return user;
        }
    }
}
