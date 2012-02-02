using System.IdentityModel.Claims;

namespace Nancy.Security
{
    public interface IUserIdentity
    {
        string UserName { get; set; }
        ClaimSet Claims { get; set; }
    }
}
