# Project authorization — implementation report

Step-by-step record of what was built, why, and how the pieces fit together.

---

## 1. Problem we were solving

After **Join** assigns `ProjectRoles.UNASSIGNED`, crew members could:

- See **Details** and **Edit** on the project dashboard
- Open those URLs directly and load sensitive project data

We needed:

1. **Enforcement** before MediatR handlers run (not only hiding buttons)
2. A path to a **role → permission** matrix (assign-role dashboard later)
3. **UI capability flags** separate from hard gates
4. Correct **DI lifetimes** (scoped EF, handlers that depend on it)

---

## 2. Design options considered

| Approach | Verdict |
|----------|---------|
| Inline `Ensure…` in every handler | Worked short-term; easy to forget on new slices |
| Custom requirement types + MediatR behavior only | Good; duplicated ASP.NET Core concepts |
| **ASP.NET Core `IAuthorizationRequirement` + MediatR `AuthorizationBehavior` + `IProjectPermissionService`** | **Chosen** — one matrix, two entry points (pipeline + optional `IAuthorizationService`) |
| Page-only `[Authorize(Policy = …)]` | Too coarse — project id lives on commands/queries |

---

## 3. Architecture (final)

```text
┌─────────────────────────────────────────────────────────────┐
│ Razor Page  →  IMediator.Send(Query|Command)                 │
└───────────────────────────────┬─────────────────────────────┘
                                ▼
┌─────────────────────────────────────────────────────────────┐
│ MediatR pipeline (outer → inner)                             │
│   CommandAudit → Performance → Validation → Authorization    │
│                                              → Handler        │
└───────────────────────────────┬─────────────────────────────┘
                                ▼
        IAuthorizableRequest.GetAuthorizationRequirements()
                                ▼
        AuthorizationBehavior → IProjectPermissionService.EnsurePermissionAsync
                                ▼
        ProjectPermissionMatrix (tier × permission)

Parallel path for future API / policy use:
        IAuthorizationService → ProjectPermissionHandler (scoped)
                              → same IProjectPermissionService
```

### Two concerns, two mechanisms

| Concern | Mechanism | Example |
|---------|-----------|---------|
| **Must not run handler** | `IAuthorizableRequest` + `AuthorizationBehavior` | `ManageProject` on Details query |
| **What UI may show** | `ProjectDashboardCapabilities` from `GetDashboardCapabilitiesAsync` | Upload form, Edit links |

Capabilities are **not** a security boundary. The pipeline is.

---

## 4. What was implemented

### 4.1 Application layer (`Application/Authorization/`, `Application/Services/`)

- **`ProjectPermission`** — enum of actions (`ViewDashboard`, `ManageProject`, `UploadDocuments`, …)
- **`ProjectAccessTier`** — `Owner`, `Manager`, `Viewer` (resolved from `OwnerId` + `ProjectRoles`)
- **`ProjectPermissionRequirement`** — implements `Microsoft.AspNetCore.Authorization.IAuthorizationRequirement`
- **`ProjectResource`** — resource type for `IAuthorizationService.AuthorizeAsync`
- **`IAuthorizableRequest`** — MediatR requests declare `GetAuthorizationRequirements()`
- **`ProjectDashboardCapabilities`** — UI DTO (`CanUploadDocuments`, `CanManageProject`, …)
- **`IProjectPermissionService`** — `HasPermissionAsync`, `EnsurePermissionAsync`, `GetDashboardCapabilitiesAsync`

### 4.2 Infrastructure

- **`ProjectPermissionMatrix`** — static tier → permission set
- **`ProjectPermissionService`** (**scoped**) — loads membership once per check; throws `NotFoundException` / `ForbiddenAccessException`
- **`ProjectPermissionHandler`** (**scoped**) — ASP.NET Core `AuthorizationHandler<ProjectPermissionRequirement, ProjectResource>`; delegates to permission service
- **`AuthorizationBehavior`** — runs after validation; calls `EnsurePermissionAsync` per requirement

### 4.3 Registration (`Extensions/AuthorizationExtensions.cs`)

```csharp
services.AddOnSetProjectAuthorization();      // scoped service + scoped handler
services.AddOnSetAuthorizationPipeline();     // MediatR behavior (register after Validation)
```

**Lifetime rule (your note):** `IProjectPermissionService` and `ProjectPermissionHandler` are **scoped** because they use `OnSetDbContext`. MediatR behaviors stay **transient** but resolve scoped services within the request scope — valid in ASP.NET Core and in integration tests that create a scope per `Send`.

We did **not** register the handler as transient with a captive dependency on scoped DbContext.

### 4.4 Permission matrix (current)

| Tier | How resolved | Permissions |
|------|----------------|-------------|
| **Owner** | `Project.OwnerId == userId` | All enum values |
| **Manager** | `UserProject.RoleOnProject == PRODUCTION` | View, manage, upload, members, delete/archive docs, schedule — **not** `DeleteProject` |
| **Viewer** | Any other member (e.g. `UNASSIGNED`, `CAMERA`, …) | `ViewDashboard` only |

Non-members: no permissions → `ForbiddenAccessException` on ensure.

### 4.5 Requests wired to authorization

| Request | Requirement |
|---------|-------------|
| `ProjectDashboard.Query` | `ViewDashboard` |
| `Details.Query` | `ManageProject` |
| `Edit.Query` / `Edit.Command` | `ManageProject` |
| `UploadDocument.Command` | `UploadDocuments` |

Join, Create, Index, etc. — no `IAuthorizableRequest` (unchanged).

### 4.6 Handlers thinned

Handlers no longer call `EnsureCanManage…` or membership checks for project auth. They focus on loading data, business rules, and mapping.

Dashboard handler sets:

```csharp
Capabilities = await _permissionService.GetDashboardCapabilitiesAsync(project.Id, userId, ct);
```

### 4.7 UI

`Pages/Projects/Project.cshtml` uses `Model.Data.Capabilities.CanManageProject` and `.CanUploadDocuments`.

### 4.8 Removed

- `IProjectAccessService` / `ProjectAccessService` (replaced by permission service + matrix)

---

## 5. Pipeline order (important)

```
Validation → Authorization → Handler
```

Requirements that need a project id are only evaluated **after** FluentValidation (e.g. `Details.Query.Id` not null). `GetAuthorizationRequirements()` returns nothing when `Id` is null so authorization does not run on invalid input.

---

## 6. Testing

- **Integration tests** use `AddOnSetProjectAuthorization()` + `AddOnSetAuthorizationPipeline()` in `MediatRIntegrationFactory`
- Auth failures surface as `ForbiddenAccessException` through MediatR (e.g. upload as `UNASSIGNED` member)
- **`ProjectPermissionServiceTests`** cover matrix and not-found behavior
- Handler unit tests focus on business logic; auth covered by integration + permission service tests

**287** tests passing after implementation.

---

## 7. Adding a new permission later

1. Add value to **`ProjectPermission`**
2. Add to **`ProjectPermissionMatrix`** for the right tier(s)
3. Add property to **`ProjectDashboardCapabilities`** if the UI needs it
4. On the MediatR request: implement **`IAuthorizableRequest`** and return `new ProjectPermissionRequirement(projectId, permission)`
5. No change to **`AuthorizationBehavior`** unless you introduce a new requirement type

For **Assign role** (future): new command with `ManageMembers` requirement; handler updates `UserProject.RoleOnProject`; matrix picks up new role automatically if tier mapping changes.

---

## 8. Optional next steps

- Call **`IAuthorizationService.AuthorizeAsync`** from Razor for early page-level checks (optional; MediatR already enforces)
- Add **`ProjectPermissionHandler`** integration test with real `ClaimsPrincipal`
- Extend **Manager** tier when assign-role UI defines more roles
- Document in **`newfeatureslice.md`** checklist: project-scoped requests → `IAuthorizableRequest`

---

## 9. File map

| Path | Role |
|------|------|
| `Application/Authorization/*` | Permissions, requirements, capabilities, `IAuthorizableRequest` |
| `Application/Services/IProjectPermissionService.cs` | Public API for checks |
| `Infrastructure/Authorization/ProjectPermissionMatrix.cs` | Tier rules |
| `Infrastructure/Services/ProjectPermissionService.cs` | EF + matrix (scoped) |
| `Infrastructure/Authorization/ProjectPermissionHandler.cs` | ASP.NET Core handler (scoped) |
| `Infrastructure/Behaviors/AuthorizationBehavior.cs` | MediatR gate |
| `Extensions/AuthorizationExtensions.cs` | DI registration |
