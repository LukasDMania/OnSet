using System.Reflection;
using Microsoft.OpenApi.Models;
using OnSet.Infrastructure.OpenApi;

namespace OnSet.Extensions;

/// <summary>
/// Registers OpenAPI (Swagger) generation for OnSet, including XML documentation and Razor Page route catalog.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swashbuckle OpenAPI generation with XML comments, MediatR schemas, and the Razor Pages operation catalog.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddOnSetSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(OnSetOpenApiMetadata.DocumentName, new OpenApiInfo
            {
                Title = OnSetOpenApiMetadata.ApiTitle,
                Version = OnSetOpenApiMetadata.ApiVersion,
                Description = OnSetOpenApiMetadata.ApiDescription,
                Contact = new OpenApiContact
                {
                    Name = "OnSet",
                },
            });

            options.EnableAnnotations();
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            options.AddSecurityDefinition(OnSetOpenApiMetadata.CookieAuthScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = OnSetOpenApiMetadata.IdentityCookieName,
                Description = "ASP.NET Core Identity application cookie. Sign in via POST /Users/Login before calling protected routes.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = OnSetOpenApiMetadata.CookieAuthScheme,
                        },
                    },
                    Array.Empty<string>()
                },
            });

            options.DocumentFilter<MediatRSchemasDocumentFilter>();
            options.DocumentFilter<RazorPagesOpenApiDocumentFilter>();
        });

        return services;
    }

    /// <summary>
    /// Exposes the OpenAPI JSON document and Swagger UI (typically in Development only).
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The same <paramref name="app"/> instance for chaining.</returns>
    public static WebApplication UseOnSetSwaggerUi(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{OnSetOpenApiMetadata.DocumentName}/swagger.json", OnSetOpenApiMetadata.ApiTitle);
            options.DocumentTitle = $"{OnSetOpenApiMetadata.ApiTitle} — OpenAPI";
            options.DisplayRequestDuration();
        });

        return app;
    }
}
