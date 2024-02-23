using BlogCMS.Core.SeedWorks.Constants;
using System.Security.Claims;

namespace BlogCMS.API.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity)claimsPrincipal.Identity).Claims.Single(x => x.Type == UserClaims.Id);
            return Guid.Parse(claim.Value);
        }
    }
}
