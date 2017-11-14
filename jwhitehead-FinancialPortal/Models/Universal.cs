using jwhitehead_FinancialPortal.Models;
using jwhitehead_FinancialPortal.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace jwhitehead_FinancialPortal.Controllers
{

    public class Universal : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuted(ActionExecutedContext filterContext) // OnActionExecuting happened
                                                                                      // at the same time as the controller was called. Changing it to OnActionExecuted happens after
        {
            base.OnActionExecuted(filterContext);
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;

                ViewBag.UserTimeZone = user.TimeZone;

                // Don't show if user does not have a household
                // Prevents error on user registration
                if (user.HouseholdId == null)
                {
                    ViewBag.OverDraft = "False";
                }
                else
                {
                    // Find current user accounts with a balance below $0.
                    List<BankAccount> currentUserAccountsOverDraft = new List<BankAccount>();
                    currentUserAccountsOverDraft = user.Household.BankAccounts.Where(b => b.Balance < 0).ToList();
                    if (currentUserAccountsOverDraft.Count() == 0)
                    {
                        ViewBag.OverDraft = "False"; // will be checked in the _Layout view
                    }
                    else
                    {
                        ViewBag.OverDraft = "True"; // will be checked in the _Layout view
                    }
                }

            }
        }
    }
}