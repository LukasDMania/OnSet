using MediatR;
using Microsoft.AspNetCore.Authorization;
using OnSet;
using OnSet.Application.Authorization;
using OnSet.Application.Exceptions;
using OnSet.Application.Services;

namespace OnSet.Infrastructure.Behaviors;

/// <summary>
/// Evaluates <see cref="IAuthorizableRequest"/> requirements via scoped <see cref="IProjectPermissionService"/> (after validation, before handler).
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IProjectPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;

    public AuthorizationBehavior(
        IProjectPermissionService permissionService,
        ICurrentUserService currentUser)
    {
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuthorizableRequest authorizable)
        {
            return await next();
        }

        var userId = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ForbiddenAccessException("User is not authenticated.");
        }

        foreach (var requirement in authorizable.GetAuthorizationRequirements())
        {
            if (requirement is ProjectPermissionRequirement permissionRequirement)
            {
                await _permissionService.EnsurePermissionAsync(
                    permissionRequirement.ProjectId,
                    userId,
                    permissionRequirement.Permission,
                    cancellationToken);
            }
        }

        return await next();
    }
}
