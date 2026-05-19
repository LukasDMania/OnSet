using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet.Application.Services;
using OnSet.Infrastructure.Authorization;
using OnSet.Infrastructure.Behaviors;
using OnSet.Infrastructure.Services;

namespace OnSet.Extensions;

/// <summary>Registers project authorization (scoped permission service, scoped ASP.NET Core handler, MediatR behavior).</summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds <see cref="IProjectPermissionService"/> (scoped), <see cref="ProjectPermissionHandler"/> (scoped),
    /// and <see cref="AuthorizationBehavior{TRequest,TResponse}"/> for MediatR.
    /// </summary>
    public static IServiceCollection AddOnSetProjectAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        // Scoped: shares DbContext lifetime with handlers and avoids captive dependencies.
        services.AddScoped<IProjectPermissionService, ProjectPermissionService>();
        services.AddScoped<IAuthorizationHandler, ProjectPermissionHandler>();

        return services;
    }

    /// <summary>Registers <see cref="AuthorizationBehavior{TRequest,TResponse}"/> (call after <see cref="ValidationBehavior{TRequest,TResponse}"/>).</summary>
    public static IServiceCollection AddOnSetAuthorizationPipeline(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        return services;
    }
}
