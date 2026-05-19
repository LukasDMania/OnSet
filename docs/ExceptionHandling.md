# Exception handling

This document describes how errors are surfaced in OnSet (Razor Pages + MediatR) and what to use when adding features.

## Layers (summary)

| Situation | Preferred approach |
|-----------|-------------------|
| Input / FluentValidation failures on commands | Return `Result.Invalid` / `Result.Fail` (handled by `ValidationBehavior` + page `ModelState`). |
| Expected “cannot do this” business outcome | Prefer `Result.Fail` for commands; for queries, throwing domain exceptions is fine if documented below. |
| Resource missing | `throw new NotFoundException(...)` |
| User not allowed | `throw new ForbiddenAccessException(...)` (or `Result.Fail` for commands—stay consistent per feature). |
| Business rule violated (domain invariant) | `throw new DomainRuleException(...)` |
| Unexpected bug (null ref, EF failure, etc.) | Do not catch in handlers; let it propagate. |

## Domain exception types

Defined under `Application/Exceptions/`:

- **`NotFoundException`** — e.g. project or user id does not exist.
- **`ForbiddenAccessException`** — authenticated user may not perform the action or view the resource.
- **`DomainRuleException`** — a stated business rule was violated (distinct from validation field errors).

## `DomainExceptionFilter`

Registered globally for Razor Pages via `AddMvcOptions` in `Program.cs`.

- It **only** handles the three types above. It does **not** catch arbitrary `Exception`s.
- If the exception is not one of those types, `ExceptionHandled` stays `false` and normal ASP.NET Core handling applies.

### User-visible behaviour

| Exception | Behaviour |
|-----------|-----------|
| `NotFoundException` | Redirect to `/NotFound` (page sets HTTP **404**). |
| `ForbiddenAccessException` | Redirect to `/Forbidden` (page sets HTTP **403**). |
| `DomainRuleException` | Redirect to `/BadRequest` (page sets HTTP **400**). In **Development** only, the exception message is copied into TempData so `/BadRequest` can show it. In **Production**, that detail is **not** shown to the user (generic message only). |

## Development vs production

- **Development** (`ASPNETCORE_ENVIRONMENT=Development`):
  - `UseDeveloperExceptionPage()` — any **unhandled** exception shows a detailed error page (stack trace, etc.).
  - Migrations and role seeding run on startup (see `Program.cs`).

- **Production** (any other environment):
  - `UseExceptionHandler("/Error")` — unhandled exceptions are redirected to the generic **Error** page (no stack trace to the client).
  - `UseHsts()` enabled.

Serilog request logging runs in both; check logs for full exception details in production.

## MediatR pipeline vs exceptions

- **`ValidationBehavior`** — validation failures typically do **not** throw; they return failed `Result` types.
- **`CommandAuditBehavior`** / **`PerformancePipelineBehavior`** — audit and timing; they do not replace exception handling.

Throwing from inside a handler still bubbles to the filter (for domain types) or to the developer/exception middleware (for everything else).

## Adding a new feature (checklist)

1. Decide: **throw** `NotFoundException` / `ForbiddenAccessException` / `DomainRuleException`, or return **`Result`**?
2. Keep the same pattern as neighbouring features in the same area (projects vs users, commands vs queries).
3. Do not catch broad `Exception` in handlers unless you are translating to `Result` or rethrowing.

## Related files

- `Infrastructure/Filters/DomainExceptionFilter.cs`
- `Pages/NotFound.cshtml` (+ code-behind)
- `Pages/Forbidden.cshtml` (+ code-behind)
- `Pages/BadRequest.cshtml` (+ code-behind)
- `Pages/Error.cshtml` — fallback for unhandled errors (production)
