using System;
using System.Security.Claims;

namespace NSE.WebApi.Core.Usuario
{
    public static class ClaimsTypeExtensions
    {
        public static string GetUserClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            if (principal == null)
                throw new ArgumentException(nameof(principal));

            return principal.FindFirstValue(claimType);
        }
    }
}

