using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace jwhitehead_FinancialPortal.Models.Helpers
{
    public class AuthorizeHouseholdRequired : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            return httpContext.User.Identity.IsInHousehold();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult
                    (new RouteValueDictionary
                    (new { controller = "Households", action = "Create" }));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext); // go to login
            }
        }
    }
}