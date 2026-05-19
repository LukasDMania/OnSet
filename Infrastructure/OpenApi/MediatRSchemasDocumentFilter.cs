using MediatR;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnSet.Infrastructure.OpenApi;

/// <summary>
/// Registers OpenAPI schemas for all MediatR <see cref="IRequest"/> and <see cref="IRequest{TResponse}"/> types in the application assembly.
/// </summary>
public sealed class MediatRSchemasDocumentFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var requestTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && !t.Name.EndsWith("Handler", StringComparison.Ordinal)
                        && !t.Name.EndsWith("Validator", StringComparison.Ordinal)
                        && t.GetInterfaces().Any(IsMediatRRequest))
            .Distinct()
            .OrderBy(t => t.FullName, StringComparer.Ordinal);

        foreach (var type in requestTypes)
        {
            context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
        }
    }

    private static bool IsMediatRRequest(Type interfaceType) =>
        interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRequest<>);
}
