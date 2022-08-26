using Shared.Models;
using System.Security.Claims;

namespace Shared.Extensions
{
    public static class ClientPrincipalExtensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this ClientPrincipal clientPrincipal)
        {
            var claimsIdentity = new ClaimsIdentity(clientPrincipal.IdentityProvider);

            claimsIdentity = new ClaimsIdentity(clientPrincipal.IdentityProvider);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, clientPrincipal.UserDetails));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, clientPrincipal.UserId));

            claimsIdentity.AddClaims(clientPrincipal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            if(clientPrincipal.Claims is not null)
            {
                foreach (var claim in clientPrincipal.Claims)
                {
                    claimsIdentity.AddClaim(new Claim(claim.Typ, claim.Val));
                }
            }            

            return claimsIdentity;
        }
    }
}
