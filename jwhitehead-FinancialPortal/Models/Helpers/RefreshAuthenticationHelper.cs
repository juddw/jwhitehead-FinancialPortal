using jwhitehead_FinancialPortal.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace jwhitehead_FinancialPortal
{
    // claims refresh -- signs you out and back in again so if you create, leave, or join a household.
    public static class RefreshAuthenticationHelper
    {
        // because this is an async method, you need async on actions that call this.
        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>()
                .SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}