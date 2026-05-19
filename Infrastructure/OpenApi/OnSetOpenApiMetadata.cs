namespace OnSet.Infrastructure.OpenApi;

/// <summary>
/// Shared OpenAPI metadata and response descriptions for OnSet.
/// </summary>
public static class OnSetOpenApiMetadata
{
    /// <summary>OpenAPI document name passed to <c>AddSwaggerGen</c>.</summary>
    public const string DocumentName = "v1";

    /// <summary>API title shown in Swagger UI.</summary>
    public const string ApiTitle = "OnSet API";

    /// <summary>Semantic API version.</summary>
    public const string ApiVersion = "v1";

    /// <summary>Security scheme id for cookie-based Identity authentication.</summary>
    public const string CookieAuthScheme = "CookieAuth";

    /// <summary>Default ASP.NET Core Identity application cookie name.</summary>
    public const string IdentityCookieName = ".AspNetCore.Identity.Application";

    /// <summary>
    /// Long-form API description including MediatR, Razor Pages, and error-page routing.
    /// </summary>
    public const string ApiDescription =
        """
        OnSet is a film-production crew and project management application built with **Razor Pages**, **MediatR**, and **ASP.NET Core Identity**.

        ## How to read this document

        - **Paths** under *Razor Pages* describe browser-facing routes (HTML). They are listed for integrators and technical writers; the live UI uses antiforgery tokens and cookies.
        - **Schemas** document MediatR **commands**, **queries**, and **view models** used by page handlers and validators.
        - **Health** is exposed at `GET /health` (not duplicated here).

        ## Authentication

        Most project and profile routes require the **Authenticated** authorization policy. Sign in via `POST /Users/Login` to receive an Identity application cookie.

        ## Domain errors (Razor Pages)

        Unhandled domain exceptions are mapped by `DomainExceptionFilter`:

        | Exception | HTTP | User-facing page |
        |-----------|------|------------------|
        | `NotFoundException` | 404 | `/NotFound` |
        | `ForbiddenAccessException` | 403 | `/Forbidden` |
        | `DomainRuleException` | 400 | `/BadRequest` |

        Command validation failures return `Result` / `Result<T>` with `IsInvalid` or `Success = false` (no redirect).

        ## Unhandled errors

        - **Development**: developer exception page with stack trace.
        - **Production**: redirect to `/Error` (HTTP 500).
        """;

    /// <summary>Tag for account-related Razor Pages.</summary>
    public const string TagUsers = "Users";

    /// <summary>Tag for project-related Razor Pages.</summary>
    public const string TagProjects = "Projects";

    /// <summary>Tag for application shell and utility pages.</summary>
    public const string TagApp = "Application";

    /// <summary>Tag for error and status pages.</summary>
    public const string TagErrors = "Errors";
}
