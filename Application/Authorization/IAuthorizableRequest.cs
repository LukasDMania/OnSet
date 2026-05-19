using Microsoft.AspNetCore.Authorization;

namespace OnSet.Application.Authorization;

/// <summary>MediatR requests that declare authorization requirements evaluated in <see cref="Infrastructure.Behaviors.AuthorizationBehavior{TRequest,TResponse}"/>.</summary>
public interface IAuthorizableRequest
{
    /// <summary>Requirements evaluated after FluentValidation and before the handler runs.</summary>
    IEnumerable<IAuthorizationRequirement> GetAuthorizationRequirements();
}
