using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HiperTrip.Extensions
{
    public static class HttpContextExtension
    {
        public static IList<string> GetUserRoles(this HttpContext context)
        {
            IList<string> roles = default;

            if (!context.IsNull())
            {
                roles = context.User.Claims
                                .Where(x => x.Type == ClaimTypes.Role)
                                .Select(x => x.Value)
                                .ToList();
            }

            return roles;
        }

        public static string GetUniqueName(this HttpContext context)
        {
            if (!context.IsNull())
            {
                return context.User.Identity.Name ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetTokenClaim(this HttpContext context, string claimType)
        {
            if (!context.IsNull())
            {
                IDictionary<string, string> claims = context.User.Claims.ToDictionary(x => x.Type, x => x.Value);

                foreach (KeyValuePair<string, string> claim in claims)
                {
                    if (claim.Key == claimType)
                    {
                        return claim.Value;
                    }
                }
            }

            return string.Empty;
        }
    }
}