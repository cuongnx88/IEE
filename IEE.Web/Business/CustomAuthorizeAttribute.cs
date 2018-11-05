using IEE.Infrastructure.DbContext;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IEE.Web.Business
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                var authorizedRoles = ConfigurationManager.AppSettings[RolesConfigKey];

                Roles = String.IsNullOrEmpty(Roles) ? authorizedRoles : Roles;

                if (!String.IsNullOrEmpty(Roles))
                {
                    if (!CurrentUser.IsInRole(Roles))
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                                                        RouteValueDictionary(new { controller = "Error", action = "AccessDenied", Areas= "ttn_content" }));
                    }
                }
            }

        }
    }
}