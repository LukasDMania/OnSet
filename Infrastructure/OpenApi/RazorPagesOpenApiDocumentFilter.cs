using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnSet.Infrastructure.OpenApi;

/// <summary>
/// Adds Razor Page routes to the OpenAPI document so Swagger UI lists the full OnSet surface area.
/// </summary>
public sealed class RazorPagesOpenApiDocumentFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var endpoint in Endpoints)
        {
            if (!swaggerDoc.Paths.ContainsKey(endpoint.Path))
            {
                swaggerDoc.Paths[endpoint.Path] = new OpenApiPathItem();
            }

            var pathItem = swaggerDoc.Paths[endpoint.Path];
            pathItem.Operations[endpoint.Method] = CreateOperation(endpoint);
        }
    }

    private static OpenApiOperation CreateOperation(PageEndpoint endpoint)
    {
        var operation = new OpenApiOperation
        {
            Summary = endpoint.Summary,
            Description = endpoint.Description,
            Tags = endpoint.Tags.Select(t => new OpenApiTag { Name = t }).ToList(),
            Deprecated = endpoint.Deprecated,
        };

        foreach (var parameter in endpoint.Parameters)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = parameter.Name,
                In = parameter.Location,
                Required = parameter.Required,
                Description = parameter.Description,
                Schema = new OpenApiSchema { Type = parameter.SchemaType },
            });
        }

        operation.Responses = CreateResponses(endpoint.Responses);
        operation.Security = endpoint.RequiresAuth
            ? new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = OnSetOpenApiMetadata.CookieAuthScheme, Type = ReferenceType.SecurityScheme } }] =
                        Array.Empty<string>(),
                },
            }
            : new List<OpenApiSecurityRequirement>();

        return operation;
    }

    private static OpenApiResponses CreateResponses(IReadOnlyList<PageResponse> responses)
    {
        var result = new OpenApiResponses();
        foreach (var response in responses)
        {
            result[response.StatusCode] = new OpenApiResponse
            {
                Description = response.Description,
            };
        }

        return result;
    }

    private static readonly IReadOnlyList<PageEndpoint> Endpoints =
    [
        Page(OperationType.Get, "/", OnSetOpenApiMetadata.TagApp, "Home", "Landing page.", auth: false, responses: Responses(Html200())),
        Page(OperationType.Get, "/Privacy", OnSetOpenApiMetadata.TagApp, "Privacy policy", "Privacy policy page.", auth: false, responses: Responses(Html200())),
        Page(OperationType.Get, "/culture/set", OnSetOpenApiMetadata.TagApp, "Set UI culture",
            "Sets the localization cookie (`culture` query: en, nl, fr) and redirects to `returnUrl` or home.",
            auth: false,
            parameters: [Query("culture", "Culture code (en, nl, fr).", required: true), Query("returnUrl", "Local return URL.", required: false)],
            responses: Responses(Redirect302(), Html200())),
        Page(OperationType.Get, "/Users/Login", OnSetOpenApiMetadata.TagUsers, "Login form", "Displays the sign-in form.", auth: false, responses: Responses(Html200())),
        Page(OperationType.Post, "/Users/Login", OnSetOpenApiMetadata.TagUsers, "Sign in",
            "Authenticates via MediatR `Features.Users.Login.Command`. On success, redirects to `returnUrl`.",
            auth: false,
            parameters: [Query("returnUrl", "Post-login redirect (local URL).", required: false)],
            responses: Responses(Html200("Validation errors re-display the form."), Redirect302("Successful sign-in."))),
        Page(OperationType.Get, "/Users/Register", OnSetOpenApiMetadata.TagUsers, "Registration form", "Displays the registration form.", auth: false, responses: Responses(Html200())),
        Page(OperationType.Post, "/Users/Register", OnSetOpenApiMetadata.TagUsers, "Register account",
            "Creates a user via MediatR `Features.Users.Register.Command`.",
            auth: false,
            responses: Responses(Html200("Validation or business errors."), Redirect302("Account created and signed in."))),
        Page(OperationType.Post, "/Users/Logout", OnSetOpenApiMetadata.TagUsers, "Sign out",
            "Signs out the current user via MediatR `Features.Users.Logout.Command`.",
            responses: Responses(Redirect302())),
        Page(OperationType.Get, "/Users/Details", OnSetOpenApiMetadata.TagUsers, "My profile",
            "Loads the signed-in user's profile via `Features.Users.Details.Query`.",
            responses: Responses(Html200(), NotFound404(), Forbidden403())),
        Page(OperationType.Post, "/Users/Details", OnSetOpenApiMetadata.TagUsers, "Update my profile",
            "Updates profile via `Features.Users.Edit.Command`.",
            responses: Responses(Html200("Validation errors."), Redirect302("Profile updated."))),
        Page(OperationType.Get, "/Users/OtherUserDetails/{id}", OnSetOpenApiMetadata.TagUsers, "View another user",
            "Loads a user profile by id via `Features.Users.OtherUserDetails.Query`.",
            parameters: [Path("id", "User identifier.", "string")],
            responses: Responses(Html200(), NotFound404(), Forbidden403())),
        Page(OperationType.Get, "/Projects", OnSetOpenApiMetadata.TagProjects, "Project list",
            "Lists projects for the current user via `Features.Projects.Index.Query`.",
            responses: Responses(Html200(), Forbidden403())),
        Page(OperationType.Post, "/Projects?handler=Join", OnSetOpenApiMetadata.TagProjects, "Join project by code",
            "Joins a project via `Features.Projects.Join.Command` (handler `Join`).",
            responses: Responses(Html200("Join code invalid or business rule failed."))),
        Page(OperationType.Get, "/Projects/Create", OnSetOpenApiMetadata.TagProjects, "Create project form", "Displays the create-project form.", responses: Responses(Html200())),
        Page(OperationType.Post, "/Projects/Create", OnSetOpenApiMetadata.TagProjects, "Create project",
            "Creates a project via `Features.Projects.Create.Command`.",
            responses: Responses(Html200("Validation errors."), Redirect302("Redirect to project list."))),
        Page(OperationType.Get, "/Projects/Details/{Id}", OnSetOpenApiMetadata.TagProjects, "Project details",
            "Project detail view via `Features.Projects.Details.Query`. Requires project membership.",
            parameters: [Path("Id", "Project identifier.", "integer")],
            responses: Responses(Html200(), NotFound404(), Forbidden403())),
        Page(OperationType.Get, "/Projects/Edit/{id}", OnSetOpenApiMetadata.TagProjects, "Edit project form",
            "Loads edit form via `Features.Projects.Edit.Query`. Owner only.",
            parameters: [Path("id", "Project identifier.", "integer")],
            responses: Responses(Html200(), NotFound404(), Forbidden403())),
        Page(OperationType.Post, "/Projects/Edit/{id}", OnSetOpenApiMetadata.TagProjects, "Update project",
            "Persists changes via `Features.Projects.Edit.Command`.",
            parameters: [Path("id", "Project identifier.", "integer")],
            responses: Responses(Html200("Validation errors."), Redirect302(), NotFound404(), Forbidden403())),
        Page(OperationType.Get, "/Projects/{Id}", OnSetOpenApiMetadata.TagProjects, "Project dashboard",
            "Dashboard via `Features.Projects.ProjectDashboard.Query`.",
            parameters: [Path("Id", "Project identifier.", "integer")],
            responses: Responses(Html200(), NotFound404(), Forbidden403())),
        Page(OperationType.Post, "/Projects/{Id}?handler=Upload", OnSetOpenApiMetadata.TagProjects, "Upload project document",
            "Uploads a file via `Features.Projects.UploadDocument.Command`.",
            parameters: [Path("Id", "Project identifier.", "integer")],
            responses: Responses(Html200("Validation errors."), Redirect302(), NotFound404())),
        Page(OperationType.Get, "/NotFound", OnSetOpenApiMetadata.TagErrors, "Not found page",
            "Returned when `NotFoundException` is thrown (HTTP 404).",
            auth: false, responses: Responses(Html404())),
        Page(OperationType.Get, "/Forbidden", OnSetOpenApiMetadata.TagErrors, "Forbidden page",
            "Returned when `ForbiddenAccessException` is thrown (HTTP 403).",
            auth: false, responses: Responses(Html403())),
        Page(OperationType.Get, "/BadRequest", OnSetOpenApiMetadata.TagErrors, "Bad request page",
            "Returned when `DomainRuleException` is thrown (HTTP 400). Development may show exception message in TempData.",
            auth: false, responses: Responses(Html400())),
        Page(OperationType.Get, "/Error", OnSetOpenApiMetadata.TagErrors, "Unhandled error page",
            "Production fallback for unhandled exceptions (HTTP 500).",
            auth: false, responses: Responses(Html500())),
    ];

    private static IReadOnlyList<PageResponse> Responses(params PageResponse[] items) => items;

    private static PageEndpoint Page(
        OperationType method,
        string path,
        string tag,
        string summary,
        string description,
        bool auth = true,
        IReadOnlyList<PageParameter>? parameters = null,
        IReadOnlyList<PageResponse>? responses = null,
        bool deprecated = false) =>
        new(method, path, [tag], summary, description, auth, parameters ?? [], responses ?? Responses(Html200()));

    private static PageParameter Query(string name, string description, bool required = false) =>
        new(name, ParameterLocation.Query, description, required, "string");

    private static PageParameter Path(string name, string description, string schemaType) =>
        new(name, ParameterLocation.Path, description, true, schemaType);

    private static PageResponse Html200(string? extra = null) =>
        new("200", $"HTML page.{(extra is null ? "" : " " + extra)}");

    private static PageResponse Redirect302(string? extra = null) =>
        new("302", $"Redirect.{(extra is null ? "" : " " + extra)}");

    private static PageResponse NotFound404() =>
        new("404", "Resource not found — see `/NotFound` (`NotFoundException`).");

    private static PageResponse Forbidden403() =>
        new("403", "Forbidden — see `/Forbidden` (`ForbiddenAccessException`).");

    private static PageResponse Html400() => new("400", "Bad request — domain rule violated (`DomainRuleException`).");
    private static PageResponse Html403() => new("403", "Forbidden.");
    private static PageResponse Html404() => new("404", "Not found.");
    private static PageResponse Html500() => new("500", "Internal server error.");

    private sealed record PageEndpoint(
        OperationType Method,
        string Path,
        IReadOnlyList<string> Tags,
        string Summary,
        string Description,
        bool RequiresAuth,
        IReadOnlyList<PageParameter> Parameters,
        IReadOnlyList<PageResponse> Responses,
        bool Deprecated = false);

    private sealed record PageParameter(
        string Name,
        ParameterLocation Location,
        string Description,
        bool Required,
        string SchemaType);

    private sealed record PageResponse(string StatusCode, string Description);
}
