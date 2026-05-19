# Adding a new feature slice

Guide for humans and LLMs implementing a vertical slice in **OnSet** (Razor Pages + MediatR CQRS + EF Core + FluentValidation).

**Read first:** [Naming conventions](NamingConventions.md) · [Exception handling](ExceptionHandling.md) · [Localization](Localization.md) · [README](../README.md) (architecture overview).

---

## Architecture (30 seconds)

```
Browser → Razor PageModel (Pages/) → IMediator.Send(Command|Query)
         → Pipeline: CommandAudit → Performance → Validation → Handler
         → OnSetDbContext / Identity / IStorageService
```

- **Commands** change state → return `Result` or `Result<T>`.
- **Queries** read data → return a `Model` (record) or throw domain exceptions.
- **PageModels** never use `OnSetDbContext` directly — only `IMediator`.
- Handlers live under `Features/<Area>/<Action>/` (one folder per use case).

**Reference slices (copy patterns from here):**

| Type | Good example |
|------|----------------|
| Command + `Result` | `Features/Projects/Create` |
| Command + file upload | `Features/Projects/UploadDocument` |
| Query + projection | `Features/Projects/Details`, `Features/Projects/Index` |
| Query + edit form | `Features/Projects/Edit` (Query + Command) |
| User / Identity | `Features/Users/Login`, `Register` |

---

## 1. Choose command vs query

| | **Command** | **Query** |
|---|-------------|-----------|
| Implements | `IRequest<Result>` or `IRequest<Result<T>>` | `IRequest<Model>` |
| Handler | `CommandHandler` | `QueryHandler` |
| Validation failure | `Result.Invalid` (via `ValidationBehavior`) | `ValidationException` (thrown) |
| Not found | `Result.Fail` **or** `NotFoundException` | `NotFoundException` |
| Forbidden | `Result.Fail` **or** `ForbiddenAccessException` | `ForbiddenAccessException` |
| Success | `Result.Ok()` / `Result<T>.Ok(value)` | return `Model` |

Match the style of **neighbouring features in the same area** (Projects vs Users).

---

## 2. Files to create

### Command slice (`Features/<Area>/<Action>/`)

```
Command.cs           — IRequest<Result> record + DataAnnotations
Validator.cs         — FluentValidation AbstractValidator<Command>
CommandHandler.cs    — IRequestHandler<Command, Result>
```

Optional: `MappingProfile.cs` only if the command returns mapped data via `Result<T>`.

### Query slice

```
Query.cs             — IRequest<Model>
Model.cs             — read model (prefer record)
MappingProfile.cs    — AutoMapper CreateProjection<Entity, Model>
QueryHandler.cs      — IRequestHandler<Query, Model>
Validator.cs         — only if query inputs need FluentValidation
```

### Razor Page (`Pages/<Area>/`)

```
<Action>.cshtml
<Action>.cshtml.cs   — <Action>Model : PageModel, inject IMediator
```

Optional: add route constant to `Utils/PageRoutes.cs` if used in redirects.

### Tests (`OnSetTests/` — mirror feature path)

```
Features/<Area>/<Action>/ValidatorTests.cs
Features/<Area>/<Action>/CommandHandlerTests.cs   (or QueryHandlerTests.cs)
Integration/Features/<Area>/<Action>IntegrationTests.cs   (when MediatR + DB + pipeline matter)
```

---

## 3. Project authorization (project-scoped slices)

Project routes use **MediatR authorization**, not ad-hoc checks in handlers.

1. Implement **`IAuthorizableRequest`** on `Query` / `Command` and return `ProjectPermissionRequirement(projectId, permission)` from `GetAuthorizationRequirements()`.
2. Pick permissions from `Application/Authorization/ProjectPermission.cs` (e.g. `ViewDashboard`, `ManageProject`, `UploadDocuments`).
3. For dashboard UI flags, set **`Model.Capabilities`** via `IProjectPermissionService.GetDashboardCapabilitiesAsync` — do not rely on capabilities alone for security.
4. Pipeline order: **Validation → Authorization → Handler** (already registered in `Program.cs`).

See [AuthorizationImplementationReport.md](AuthorizationImplementationReport.md) for the full matrix, DI lifetimes, and design rationale.

---

## 4. Implement the slice

### 3.1 `Command.cs` / `Query.cs`

- Namespace: `OnSet.Features.<Area>.<Action>`.
- Use `record` for commands; queries can be `class` or `record`.
- Add **XML docs** on the type (summary + `<remarks>` with route and related types):

```csharp
/// <summary>Creates a new project for the current user.</summary>
/// <remarks>POST <c>/Projects/Create</c> via <see cref="Pages.Projects.CreateModel"/>; validated by <see cref="Validator"/>.</remarks>
public record Command : IRequest<Result> { ... }
```

- Put `[Required]`, `[StringLength]`, `[DataType]` on command properties (helps OpenAPI schemas and display metadata).

### 3.2 `Validator.cs`

- One `Validator : AbstractValidator<Command>` (or `Query`) per request type.
- Class-level summary: `/// <summary>FluentValidation rules for this feature slice.</summary>`
- Rules for all input constraints; avoid duplicating the same checks in the handler.
- `ValidationBehavior` turns failures into `Result.Invalid` for commands automatically.

### 3.3 `CommandHandler.cs`

- Summary: `/// <summary>MediatR handler for this feature slice.</summary>`
- Inject only what you need (`OnSetDbContext`, `ICurrentUserService`, `IStorageService`, `IMapper`, `IMediator`, Identity managers, …).
- Use domain factories (`Project.Create`, `User.Create`) — do not construct entities with invalid state.
- Auth: read `ICurrentUserService.UserId`; check `UserProjects` / `OwnerId` as in sibling handlers.
- Prefer `Result.Fail("message")` for expected business failures on commands.
- Throw `NotFoundException`, `ForbiddenAccessException`, `DomainRuleException` when that matches sibling queries/commands.
- Publish side effects via `IMediator.Publish` (see `Application/Notifications/Projects/ProjectCreatedNotification.cs`).
- Call `SaveChangesAsync` once per unit of work.

### 3.4 `QueryHandler.cs`

- Use `.AsNoTracking()` for read-only queries.
- Prefer `ProjectTo<Model>(_mapper.ConfigurationProvider)` over manual mapping.
- Throw `NotFoundException` / `ForbiddenAccessException` when appropriate (see `Features/Projects/Details/QueryHandler.cs`).

### 3.5 `Model.cs` + `MappingProfile.cs`

- `Model` is a **record** with init properties.
- `MappingProfile : Profile` with `CreateProjection<Entity, Model>()`; map value objects (e.g. `Address`) explicitly.

### 3.6 Razor PageModel

```csharp
[Authorize(Policy = "Authenticated")]   // when login required
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;
    [BindProperty] public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _mediator.Send(new Command { ... });
        if (!result.Success)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return Page();
        }
        return RedirectToPage(PageRoutes.ProjectsIndex);  // or RedirectToPage("Details", new { id })
    }
}
```

- Map `Input` → `Command` in the POST handler (keep Page `InputModel` separate from MediatR types).
- Queries: `var model = await _mediator.Send(new Query { ... });` in `OnGetAsync`.
- **Do not** inject `OnSetDbContext` into PageModels.
- XML on PageModel: `/// <summary>Razor Page handler (documented in OpenAPI under Razor Pages).</summary>`

### 3.7 Localization (UI strings)

- User-visible text in `.cshtml`: `@L["Key"]` via `Resources/Resource.resx` (+ `Resource.nl.resx`).
- Handler/validation messages are still mostly English unless you extend localization (see [Localization.md](Localization.md)).

---

## 5. OpenAPI / Swagger

MediatR request types appear in **Schemas** automatically (`MediatRSchemasDocumentFilter`).

For new Razor routes, add entries to the `Endpoints` array in:

`Infrastructure/OpenApi/RazorPagesOpenApiDocumentFilter.cs`

Use existing `Page(...)` helpers; pick tag from `OnSetOpenApiMetadata` (`TagProjects`, `TagUsers`, …). Mention the MediatR type in the description, e.g. `` `Features.Projects.Foo.Command` ``.

Swagger UI is Development-only; `GET /swagger/v1/swagger.json` requires auth.

**Tests:** Do **not** add Swashbuckle packages to `OnSetTests` — they conflict with OnSet’s Swashbuckle 6.x / Microsoft.OpenApi 1.x and break `assembly.GetTypes()` during MediatR registration.

---

## 6. XML documentation

OnSet builds with `<GenerateDocumentationFile>true</GenerateDocumentationFile>` — undocumented public APIs warn as CS1591 (suppressed globally, but still add docs for new surface).

| Target | What to document |
|--------|------------------|
| `Command` / `Query` | Summary, route, PageModel, Validator |
| Handler | `MediatR handler for this feature slice.` |
| `Validator` | `FluentValidation rules for this feature slice.` |
| `MappingProfile` | `AutoMapper profile for this feature slice.` |
| PageModel | Route purpose; link to OpenAPI |
| New public services/DTOs | Summary + param/returns where helpful |

Use `<see cref="..."/>` to cross-link types. Rebuild to refresh `OnSet.xml` consumed by Swagger.

---

## 7. Unit tests (OnSetTests)

**Framework:** MSTest (`[TestClass]`, `[TestMethod]`).  
**Namespace:** `OnSet.Tests.Features.<Area>.<Action>` (mirror `Features/`).

### ValidatorTests

- Instantiate `Validator`, call `Validate(command)`.
- Naming: `Validate_When<Condition>_HasError` / `Validate_WithValidCommand_HasNoErrors`.
- Use `ValidCommand()` helper + `with { Property = value }` for variations.

### CommandHandlerTests / QueryHandlerTests

- DB: `TestDbContextFactory.CreateContext(out var connection)` (SQLite in-memory).
- Mock `ICurrentUserService`, `IMediator`, `IStorageService` with Moq as needed.
- Naming: `Handle_When<Scenario>_<ExpectedOutcome>`.
- Assert DB state (`context.Projects.Count()`, etc.) and `Result.Success` / exceptions.

### What to cover

- Happy path persists correct data.
- Unauthenticated / wrong user / not member.
- Validation edge cases (in `ValidatorTests`, not only integration).
- `Result.Fail` messages for business rules.

---

## 8. Integration tests (OnSetTests)

Use when the full MediatR pipeline matters (validation behavior, real handlers, SQLite).

- Base: `IntegrationTestBase` (or area base like `ProjectsIntegrationTestBase`).
- `[TestInitialize]` builds provider via `MediatRIntegrationFactory` (do not duplicate setup).
- Pattern:

```csharp
using var scope = CreateScope();
var db = GetDb(scope);
var mediator = GetMediator(scope);
await ProjectSeedData.AddUserAsync(db, "owner-1", "Jane", "Doe");
CurrentUser.UserId = "owner-1";
var result = await mediator.Send(command);
```

- Naming: `Send_When<Scenario>_<Outcome>`.
- Assert `result.IsInvalid` for validation failures on commands.
- Seed helpers: `ProjectSeedData`, `UserSeedData` in `Integration/TestSupport/`.

Command-audit-specific tests use `MediatRIntegrationOptions` with `IncludeCommandAuditBehavior = true` (see `CommandAuditPipelineIntegrationTests`).

---

## 9. Database & domain

- New entity: `Domain/Models/`, configure in `Infrastructure/Data/OnSetDbContext.cs`, add migration (`dotnet ef migrations add ...` from OnSet project).
- Enforce invariants in domain methods; handlers orchestrate only.
- Enum values: **ALL_CAPS** (`ONGOING`, `DIRECTOR`).

---

## 10. Code style (must match)

- **Allman braces** — opening `{` on its own line; always brace `if`/`else`/`foreach`.
- **PascalCase** types/methods; **camelCase** locals/parameters.
- Comments explain **why**, not what the next line does.
- No Swashbuckle/OpenAPI packages in test projects.

---

## 11. End-to-end checklist

Copy and tick when adding a slice:

```
[ ] Features/<Area>/<Action>/ — Command or Query + Handler + Validator (+ Model/MappingProfile)
[ ] XML docs on request type, handler, validator
[ ] IAuthorizableRequest + ProjectPermissionRequirement on project Query/Command
[ ] Result vs exception choice matches neighbouring features
[ ] Pages/<Area>/<Action>.cshtml + PageModel → IMediator only
[ ] PageRoutes constant if redirected to from multiple places
[ ] RazorPagesOpenApiDocumentFilter entry for new routes
[ ] Resource.resx keys for new UI strings (+ nl if applicable)
[ ] OnSetTests/Features/.../ValidatorTests.cs
[ ] OnSetTests/Features/.../CommandHandlerTests.cs or QueryHandlerTests.cs
[ ] OnSetTests/Integration/...IntegrationTests.cs (if pipeline/DB integration needed)
[ ] dotnet test OnSetTests — all pass
[ ] EF migration if schema changed
```

---

## 12. Common mistakes

| Mistake | Fix |
|---------|-----|
| Validation in handler instead of `Validator` | Move rules to FluentValidation |
| `throw` on command validation errors | Let `ValidationBehavior` return `Result.Invalid` |
| DbContext in Razor Page | Use MediatR only |
| Adding Swashbuckle 10.x to OnSetTests | Remove; rely on OnSet project reference |
| Magic redirect strings | `PageRoutes` or `RedirectToPage("Name", routeValues)` |
| Skipping OpenAPI catalog | Add route to `RazorPagesOpenApiDocumentFilter` |
| Inconsistent forbidden handling | Copy Projects or Users pattern for that verb |

---

## 13. Run tests

```bash
dotnet test ../OnSetTests/OnSetTests.csproj
dotnet test ../OnSetTests/OnSetTests.csproj --filter "FullyQualifiedName~Integration"
```

When changing only one slice, filter by feature name, e.g. `--filter "FullyQualifiedName~Projects.Create"`.
