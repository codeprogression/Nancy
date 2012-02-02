using System.Collections.Generic;
using System.IdentityModel.Claims;
using Nancy.Security;

namespace Nancy.Demo.Authentication.Basic
{
    public class DemoUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }

        public ClaimSet Claims { get; set; }
    }
}