using System.IdentityModel.Claims;

namespace Nancy.Demo.Authentication
{
    public class DemoClaims
    {
        public static Claim SuperSecureRole = new Claim("Role", "SuperSecureRole", Rights.Identity );
    }
}