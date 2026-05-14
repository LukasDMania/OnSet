# Naming and code style conventions

Conventions used in the OnSet codebase. When in doubt, match existing files in the same folder.

## Naming

| Element | Convention | Example |
|---------|------------|---------|
| Classes, records, structs, interfaces | **PascalCase** | `Project`, `CommandHandler`, `IStorageService` |
| Public members (methods, properties, events) | **PascalCase** | `SaveChangesAsync`, `UserId` |
| Parameters, local variables, private fields | **camelCase** | `projectId`, `cancellationToken` |
| Enum **types** | **PascalCase** | `DocumentTags`, `ProjectStatus` |
| Enum **members** (values) | **ALL_CAPS** | `CALLSHEET`, `SCENARIO`, `OTHER` |
| Namespaces | **PascalCase**, match folder structure | `OnSet.Features.Projects.Create` |
| Razor PageModel classes | **PascalCase** + `Model` suffix | `RegisterModel`, `DetailsModel` |

## Braces and control flow

### Allman style

Opening braces go on **their own line** after the control statement (not on the same line as `if` / `for` / `while`).

```csharp
if (condition)
{
    DoWork();
}
```

Same for `else`, `for`, `foreach`, `while`, `using`, `try` / `catch` / `finally`, and type declarations:

```csharp
else
{
    DoOther();
}

foreach (var item in items)
{
    Process(item);
}

try
{
    Run();
}
catch (InvalidOperationException ex)
{
    Log(ex);
}
```

### Full `if` form and always use braces

- Write the **full** `if` with a clear condition in parentheses.
- **Always** use a braced block for `if` / `else` / `else if` — do not omit braces for single-line bodies.

**Avoid:**

```csharp
if (x) return;
if (y) { Do(); }  // same-line brace (K&R) — not used in this project
```

**Prefer:**

```csharp
if (x)
{
    return;
}

if (y)
{
    Do();
}
```

## Files and folders (vertical slices)

- Feature folders: `Features/<Area>/<Action>/` with `Command.cs`, `Query.cs`, `CommandHandler.cs`, `QueryHandler.cs`, `Validator.cs`, etc.
- Names align with behaviour (verbs): `Create`, `Edit`, `UploadDocument`, `Index`.

## Comments

Use comments where the **why** is not obvious from the code; avoid restating what the code already says line by line.

## Other languages

- **Razor** (`.cshtml`): follow the same brace style in C# blocks (`@{ }`, `@code { }`) where applicable.
- **JSON** / **appsettings**: standard JSON naming (often **PascalCase** for .NET configuration sections as in Serilog examples).
