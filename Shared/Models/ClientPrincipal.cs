using System.Security.Claims;

namespace Shared.Models
{
    public class ClientPrincipal
    {
        public string IdentityProvider { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserDetails { get; set; } = null!;
        public IEnumerable<string> UserRoles { get; set; } = null!;
        public IEnumerable<ClientPrincipalClaim>? Claims { get; set; }
    }

    public class ClientPrincipalClaim
    {
        public string Typ { get; set; } = null!;
        public string Val { get; set; } = null!;
    }
}
