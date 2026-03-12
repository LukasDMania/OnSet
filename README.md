## OnSet

OnSet is a production management web application for film projects, built with ASP.NET Core Razor Pages, EF Core, and MediatR using a CQRS-style architecture. This document describes the architecture, naming conventions, and development practices used in the project. It is intended for both human developers and AI agents working on new features.

---

## Tech Stack

- **Runtime**: .NET 8
- **Web framework**: ASP.NET Core Razor Pages
- **Persistence**: Entity Framework Core (SQL Server)
- **Identity**: ASP.NET Core Identity (`User` extends `IdentityUser`)
- **CQRS / Orchestration**: MediatR
- **Validation**: FluentValidation (integrated as a MediatR pipeline behavior)
- **Object mapping**: AutoMapper
- **UI**: Razor Pages + Bootstrap 5, dark theme (`data-bs-theme="dark"`)
- **File storage**:
  - Local file system via `LocalStorageService` in development
  - Abstracted behind `IStorageService` for future Azure Blob Storage or other providers

---

## High-Level Architecture

The solution follows a layered architecture with clear separation of concerns:

- **Domain**: Core business concepts and rules.
- **Application**: Use cases, CQRS features, service interfaces, and application-level exceptions.
- **Infrastructure**: Data access, external services, and framework-specific implementations.
- **UI**: Razor Pages (presentation and request handling).

### Project Structure (Logical)

- `Domain/`
  - `Domain/Models`: Entities and aggregates, e.g. `Project`, `User`, `Document`, `Contract`, `UserProject`, etc.
  - `Domain/Enums`: Domain enums, e.g. `ProjectStatus`, `ProjectRoles`, `Languages`, `DocumentTags`.
  - `Domain/ValueObjects`: Value objects such as `Address`, `FileMetadata`, `FirstName`, `LastName`, `EmergencyContact`, etc.
  - **Rules**:
    - Contains **no** infrastructure or framework dependencies (except minimal data annotations).
    - Encapsulates domain invariants via constructors and methods (e.g. `Project.Create`, `Project.UpdateDetails`).

- `Application/`
  - `Application/Exceptions`: Cross-cutting application exceptions:
    - `NotFoundException`
    - `ForbiddenAccessException`
    - `DomainRuleException`
    - `EnumConversionException`
  - `Application/Services`:
    - `ICurrentUserService`
    - `IStorageService`
  - **Purpose**:
    - Defines use-case level contracts and errors.
    - Does not depend on Infrastructure or UI; it is referenced by them.

- `Infrastructure/`
  - `Infrastructure/Persistence/OnSetDbContext.cs`:
    - EF Core DbContext for:
      - `Projects`, `Documents`, `Contracts`, `UserProjects`
      - ASP.NET Identity `User` (inherits `IdentityDbContext<User>`)
    - Configures owned types (e.g. `User.FirstName`, `User.HomeAddress`, `Project.Location`).
    - Configures value conversions for list properties:
      - `User.SpokenLanguages`: `List<Languages>` stored as JSON.
      - `Document.Tags`: `List<DocumentTags>` stored as JSON.
  - `Infrastructure/Behaviors/ValidationBehavior.cs`:
    - MediatR pipeline behavior that runs all `IValidator<TRequest>` instances before handlers.
    - For commands returning `Result`/`Result<T>`, transforms validation errors into `Result.Invalid`.
    - For other responses, keeps exception-based flow.
  - `Infrastructure/Results/Result.cs`:
    - `Result` and `Result<T>` types with helpers:
      - `Ok()`, `Fail(...)`, `Invalid(...)`.
  - `Infrastructure/Services`:
    - `CurrentUserService` (implements `ICurrentUserService` via `IHttpContextAccessor`).
    - `LocalStorageService` (implements `IStorageService` using `IWebHostEnvironment` and `wwwroot/uploads`).
    - `Services/Email/NoOpEmailSender` (implements `IEmailSender` for logging-only email).

- `Features/`
  - Organized by **bounded feature and action**, e.g.:
    - `Features/Projects/Index`
    - `Features/Projects/Create`
    - `Features/Projects/Edit`
    - `Features/Projects/Details`
    - `Features/Projects/Join`
    - `Features/Projects/ProjectDashboard`
    - `Features/Projects/UploadDocument`
    - `Features/Users/Details`, `Edit`, `Register`, `Login`, `Logout`, `OtherUserDetails`, etc.
  - Each feature typically contains:
    - `Query.cs` or `Command.cs` — request type implementing `IRequest<TResponse>`.
    - `QueryHandler.cs` / `CommandHandler.cs` — MediatR handlers implementing `IRequestHandler<,>`.
    - `Model.cs` — read model/DTO returned to UI.
    - `MappingProfile.cs` — AutoMapper profile (often using `CreateProjection`).
    - `Validator.cs` — FluentValidation validators for the request type.

- `Pages/`
  - Razor Pages for UI; grouped by area:
    - `Pages/Projects`: `Index`, `Create`, `Edit`, `Details`, `Project` (dashboard).
    - `Pages/Users`: `Login`, `Register`, `Details`, `OtherUserDetails`, `Logout`.
    - Root `Pages/Index` and supporting pages like `Privacy`, `Error`.
  - PageModels inject `IMediator` and call appropriate CQRS features.
  - Authorization is enforced via attributes and application-level checks.

---

## Naming & Coding Conventions

### General C# Conventions

- **Namespaces**: Root namespace is `OnSet`. Sub-namespaces mirror folder structure where useful (e.g. `OnSet.Features.Projects.Index`).
- **Classes**: PascalCase (e.g. `ProjectModel`, `OnSetDbContext`).
- **Interfaces**: PascalCase with `I` prefix (e.g. `ICurrentUserService`, `IStorageService`).
- **Enums**: PascalCase type name, **ALL_CAPS** values, e.g.:
  - `public enum DocumentTags { CALLSHEET, SCENARIO, SCHEDULE, LOGISTICS, OTHER }`
- **DTOs and Models**: Use `record` types for immutable read models (e.g. `public record Model`, `public record ProjectListItem`).

### Feature Folders

- For each business action, create a folder under `Features/<Area>/<FeatureName>`:
  - **Example**: `Features/Projects/Index`:
    - `Query.cs`
    - `Model.cs`
    - `MappingProfile.cs`
    - `QueryHandler.cs`
  - **Example**: `Features/Projects/UploadDocument`:
    - `Command.cs`
    - `Validator.cs`
    - `CommandHandler.cs`
- Prefer **verb-based** or **action-based** names: `Index`, `Details`, `Create`, `Edit`, `ProjectDashboard`, `UploadDocument`, `Join`.

### CQRS + MediatR

- **Queries**:
  - `Query.cs` implements `IRequest<Model>` where `Model` is a read model.
  - `QueryHandler.cs` implements `IRequestHandler<Query, Model>`.
  - Use EF Core with `.AsNoTracking()` and AutoMapper `ProjectTo<Model>()` where possible.
  - Throw:
    - `NotFoundException` when the requested resource does not exist.
    - `ForbiddenAccessException` when the current user is not allowed to access it.

- **Commands**:
  - `Command.cs` implements `IRequest<Result>` or `IRequest<Result<T>>`.
  - `CommandHandler.cs` implements `IRequestHandler<Command, Result>` (or `Result<T>`).
  - On *validation errors*:
    - Use FluentValidation + `ValidationBehavior` to return `Result.Invalid` or `Result.Fail` (do **not** throw).
  - On *domain rule violations* or *not found*:
    - Throw `DomainRuleException` or `NotFoundException`.
  - On *forbidden* behavior:
    - Throw `ForbiddenAccessException` or return a failed `Result` for commands, depending on the context.
  - `Result` flow:
    - `Result.Ok()` for success.
    - `Result.Fail("message")` or `Result.Invalid(errors)` for failures.

### Razor Pages

- PageModel classes are named `<PageName>Model`, e.g. `IndexModel`, `EditModel`, `ProjectModel`.
- Use `[Authorize(Policy = "Authenticated")]` on PageModels that require login.
- Inject `IMediator` via constructor and call:
  - `await _mediator.Send(new Query { ... });` in `OnGet*` handlers.
  - `await _mediator.Send(command);` in `OnPost*` handlers.
- For commands returning `Result`:
  - If `!result.Success`, iterate errors and add them to `ModelState`:
    - `ModelState.AddModelError(string.Empty, error);`
  - Return `Page()` to show validation/errors.
- Use tag helpers (`asp-page`, `asp-route-*`) for navigation, not string URLs.

### Validation

- Use **FluentValidation** for request validation:
  - Define one `Validator` class per `Command` or `Query` that needs validation.
  - Example:
    - `Features/Projects/Create/Validator.cs`
    - `Features/Projects/UploadDocument/Validator.cs`
  - Avoid ad-hoc `ModelState` checks in handlers; centralize rules in validators where possible.

---

## Domain Concepts & Important Models

### Project

- Located in `Domain/Models/Project.cs`.
- Key fields:
  - `Name`, `Description`, `ClientName`, `ReferenceCode`
  - `StartDate`, `EndDate`
  - `Budget`, `ActualCost`
  - `Status` (`ProjectStatus`)
  - `OwnerId` + navigation to `Owner` (`User`)
  - `UserProjects` — membership and role per project
  - `Documents` — related `Document` entities
- Creation and update:
  - `Project.Create(...)` — enforces non-empty name, valid dates, non-negative budget, associates the creator as a member (`UserProject`).
  - `UpdateDetails(...)` — enforces domain rules for updates and sets `UpdatedAt`.

### User & UserProject

- `Domain/Models/User.cs` extends `IdentityUser` with profile and meta fields.
- `Domain/Models/UserProject.cs` links users to projects and expresses their `ProjectRoles` on that project:
  - Composite key: `{UserId, ProjectId}`.
  - Factory methods `Create(...)` for membership.

### Document & DocumentTags

- `Domain/Models/Document.cs` represents a file stored on the server:
  - `ProjectId`, `UserId`
  - `FileMetadata Metadata` (file name, extension, size, MIME type).
  - `FilePath` — storage path or URL for download.
  - `UploadedAt`, `Description`, `IsArchived`
  - `Tags` (List of `DocumentTags`), persisted as JSON via EF Core.
- `Domain/Enums/DocumentTags.cs`:
  - `CALLSHEET`, `SCENARIO`, `SCHEDULE`, `LOGISTICS`, `OTHER`.

---

## Key Features & Flows

### Projects Overview (`/Projects`)

- Feature: `Features/Projects/Index`
- Page: `Pages/Projects/Index.cshtml` + `IndexModel`
- Behavior:
  - Lists projects the current user is a member of, via `UserProjects`.
  - Only displays projects the user has joined or created.
  - From this page, the user can:
    - Create a project (`/Projects/Create`).
    - Join a project by code via `Features/Projects/Join`.

### Project Details (`/Projects/Details/{id}`) vs Dashboard (`/Projects/{id}`)

- **Details**:
  - Feature: `Features/Projects/Details`
  - Page: `Pages/Projects/Details.cshtml`
  - Shows project metadata (financials, timeline, location, owner, etc.).
  - Handler enforces membership via `UserProjects` and throws `ForbiddenAccessException` if user is not a member.

- **Dashboard (Index for a single project)**:
  - Feature: `Features/Projects/ProjectDashboard`
  - Page: `Pages/Projects/Project.cshtml` (route `/Projects/{Id:int}`).
  - Shows:
    - Project header (name, reference code, client).
    - Quick links to **Details** and **Edit** pages.
    - Grouped documents for the project (by `DocumentTags`), sorted by `UploadedAt` descending.
  - Only visible to project members (membership check in handler).

### Document Uploads

- Feature: `Features/Projects/UploadDocument`
  - `Command`:
    - `ProjectId`, `Tag` (`DocumentTags`), optional `Description`, `IFormFile File`.
  - `Validator`:
    - Ensures `ProjectId > 0`, file is non-null and non-empty.
  - `CommandHandler`:
    - Uses `ICurrentUserService` to get the current user.
    - Validates:
      - User is authenticated.
      - User is a member of the project.
      - User is the project **creator** (`OwnerId`) — current policy.
    - Uses `IStorageService.UploadFileAsync` to store the file under `uploads/projects/{ProjectId}`.
    - Creates a `Document` with metadata and selected tag.
    - Returns `Result.Ok()` on success.
    - Comments explicitly mark the logic as **open for extension**, so future work can allow specific project roles or configurable uploader lists.

- UI integration:
  - Implemented in `Pages/Projects/Project.cshtml` within the dashboard:
    - Upload form rendered only when `Model.Data.CanUploadDocuments` (project creator).
    - Form posts to `OnPostUploadAsync` in `ProjectModel`.
    - Successful upload redirects back to `/Projects/{id}`, showing the new document in the correct group.
    - Non-creators see a badge indicating uploads are managed by the production office.

---

## Authorization & Security

- Global policy:
  - `builder.Services.AddAuthorization(options => options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser()));`
  - Pages that require user login use `[Authorize(Policy = "Authenticated")]`.

- Project-level authorization:
  - Queries and commands dealing with specific projects check membership:
    - Via `UserProjects` table and `ICurrentUserService.UserId`.
  - Typical checks:
    - Membership required → throw `ForbiddenAccessException` (for queries) or return `Result.Fail` (for commands) if unauthorized.
    - Ownership required (e.g. uploads) → compare `Project.OwnerId` with current user id.

---

## Services & Dependency Injection

- Registered in `Program.cs`:
  - `OnSetDbContext` (SQL Server).
  - Identity with `User` + `IdentityRole`.
  - MediatR scanning assembly (handlers, validators, behaviors).
  - AutoMapper scanning assembly.
  - FluentValidation validators from assembly.
  - Validation pipeline behavior: `ValidationBehavior<,>`.
  - `IEmailSender` → `NoOpEmailSender`.
  - `IHttpContextAccessor`.
  - Application services:
    - `ICurrentUserService` → `CurrentUserService`.
    - `IStorageService` → `LocalStorageService`.

- Adding a new service:
  1. Define interface in `Application/Services` (e.g. `INotificationService`).
  2. Implement it under `Infrastructure/Services/...`.
  3. Register implementation in `Program.cs` (e.g. `AddScoped<INotificationService, EmailNotificationService>();`).

---

## UI & Styling

- Layout: `Pages/Shared/_Layout.cshtml`.
  - Uses Bootstrap 5.
  - Dark theme enabled via `data-bs-theme="dark"` on `<html>`.
- App styling: `wwwroot/css/site.css`.
  - Contains:
    - Base typography and layout.
    - Dark mode overrides for body, cards, tables, etc.
    - Specific tweaks:
      - `html[data-bs-theme="dark"] .table { --bs-table-color: #ffffff; ... }` to ensure table text is readable.
- Page-specific styles:
  - Some pages (e.g. project details, dashboard) add page-local `<style>` sections for specific visual refinements. Prefer reusing shared styles where reasonable.

---

## Adding New Features – Checklist

When adding a new feature, follow this checklist to stay aligned with the current architecture:

1. **Identify the area**:
   - Projects (`Features/Projects/...`) or Users (`Features/Users/...`) or another domain.

2. **Create CQRS artifacts** under `Features/<Area>/<FeatureName>`:
   - `Query.cs` or `Command.cs` (implementing `IRequest<TResponse>`).
   - `Model.cs` as needed for read models.
   - `MappingProfile.cs` if mapping/projection is non-trivial.
   - `QueryHandler.cs` or `CommandHandler.cs`.
   - `Validator.cs` with FluentValidation rules.

3. **Enforce authorization**:
   - Use `[Authorize(Policy = "Authenticated")]` when the page requires login.
   - In handlers, check membership/ownership using `ICurrentUserService` and `UserProjects`.
   - Throw `ForbiddenAccessException` or return appropriate `Result` failures as per existing patterns.

4. **Use the Result pattern** for commands:
   - Return `Result`/`Result<T>`.
   - Map validation failures to `Result.Invalid` or `Result.Fail`.
   - Only throw exceptions for not found/forbidden/domain rule violations.

5. **Wire Razor Page**:
   - Add or update a page in `Pages/...`.
   - Inject `IMediator`, call `Send` within `OnGet*` / `OnPost*`.
   - Propagate `Result.Errors` into `ModelState`.
   - Use tag helpers for navigation and form binding.

6. **Respect layering**:
   - Do not access `OnSetDbContext` directly from Razor PageModels.
   - Keep infrastructure-specific code in `Infrastructure`, not in `Domain` or `Application`.

7. **Add tests (if applicable)**:
   - Follow solution’s testing approach when present (not covered in this file).

---

## Future-Proofing Notes

- **File storage**:
  - Currently uses `LocalStorageService` writing into `wwwroot/uploads`.
  - Because the rest of the code depends only on `IStorageService`, swapping to Azure Blob Storage or another provider only requires:
    - Adding a new implementation in `Infrastructure/Services`.
    - Updating DI registration in `Program.cs`.

- **Role-based project permissions**:
  - At the moment, several features (e.g. document upload) are restricted to the project creator (`OwnerId`).
  - The architecture is intentionally open to:
    - Add richer authorization rules based on `ProjectRoles`.
    - Add project-level configuration for “uploaders” or other permissions.

---

## Getting Started (Development)

1. **Configure connection string** in `appsettings.json` or user secrets:
   - Key: `"ConnectionStrings:DefaultConnection"`.
2. **Run EF migrations**:
   - The app applies migrations automatically in development via `app.ApplyMigrations()` inside `Program.cs` (when `IsDevelopment`).
3. **Run the application**:
   - `dotnet run` from the project directory.
4. **Seeded roles**:
   - In development, roles `Admin` and `StandardUser` are ensured to exist on startup.

---

## Contact and Contributions

- Follow the conventions in this README when:
  - Adding new features.
  - Refactoring existing features.
  - Introducing new services or integrations.
- When in doubt, look at:
  - `Features/Projects/Index`, `Create`, `Details`, `ProjectDashboard`, `UploadDocument`.
  - `Features/Users/Details`, `Register`, `Login`.
  - `Pages/Projects` and `Pages/Users` implementations.

This structure should provide a solid foundation for an enterprise-style ASP.NET Core application and make it straightforward for new contributors (human or AI) to extend the system safely and consistently.

