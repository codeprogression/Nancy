namespace Nancy.Demo.Authentication
{
    using System.Linq;
    using Nancy;
    using Nancy.Demo.Authentication.Models;
    using Nancy.Security;

    /// <summary>
    /// A module that only people with SuperSecure clearance are allowed to access
    /// </summary>
    public class VerySecureModule : NancyModule
    {
        public VerySecureModule() : base("/superSecureToo")
        {
            this.RequiresValidatedClaims(c => c.Contains(DemoClaims.SuperSecureRole));

            Get["/"] = x => {
                var model = new UserModel(Context.CurrentUser.UserName);
                return View["superSecure.cshtml", model];
            };
        }
    }
}