using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace OnSet
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

                    if (user != null)
                    {
                        Claim idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                        if (idClaim != null)
                        {
                            return idClaim.Value;
                        }
                    }
                }

                return string.Empty;
            }
        }

        public string UserName
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

                    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                    {
                        return user.Identity.Name;
                    }
                }

                return string.Empty;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

                    if (user != null && user.Identity != null)
                    {
                        return user.Identity.IsAuthenticated;
                    }
                }

                return false;
            }
        }

        public List<string> Roles
        {
            get
            {
                List<string> roles = new List<string>();

                if (_httpContextAccessor.HttpContext != null)
                {
                    ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;

                    if (user != null)
                    {
                        IEnumerable<Claim> roleClaims = user.FindAll(ClaimTypes.Role);

                        foreach (Claim claim in roleClaims)
                        {
                            roles.Add(claim.Value);
                        }
                    }
                }

                return roles;
            }
        }

        public string? ClientIp =>
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public string? CorrelationId
        {
            get
            {
                var http = _httpContextAccessor.HttpContext;
                if (http is null)
                    return null;

                if (http.Request.Headers.TryGetValue("X-Correlation-Id", out StringValues header))
                {
                    var fromHeader = header.FirstOrDefault(h => !string.IsNullOrWhiteSpace(h));
                    if (!string.IsNullOrWhiteSpace(fromHeader))
                        return fromHeader.Trim();
                }

                return http.TraceIdentifier;
            }
        }
    }
}

