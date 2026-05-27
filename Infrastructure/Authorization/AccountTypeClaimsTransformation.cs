using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;

namespace OnSet.Infrastructure.Authorization
{
    /// <summary>Adds AccountType claim based on the current user record.</summary>
    public sealed class AccountTypeClaimsTransformation : IClaimsTransformation
    {
        public const string ClaimType = "onset:account_type";

        private readonly UserManager<User> _userManager;

        public AccountTypeClaimsTransformation(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return principal;
            }

            if (principal.HasClaim(c => c.Type == ClaimType))
            {
                return principal;
            }

            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                return principal;
            }

            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return principal;
            }

            identity.AddClaim(new Claim(ClaimType, user.AccountType.ToString()));
            return principal;
        }
    }
}

