using jwhitehead_FinancialPortal.Models.Helpers;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace jwhitehead_FinancialPortal.Controllers
{
    public class HomeController : Universal
    {
        // GET: Assigned Projects
        private HouseholdAssignHelper helper = new HouseholdAssignHelper();

        // GET: Households
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) // user is logged in.
            {
                ViewBag.UserTimeZone = db.Users.Find(User.Identity.GetUserId()).TimeZone;
                var userId = User.Identity.GetUserId();

                var usersHousehold = db.Households.Any(u => u.AuthorId == userId); // check if user has a household
                if (usersHousehold == false)
                {
                    return RedirectToAction("NoHouseHold");
                }
                else
                {
                    return View(); // user has a household so return to the view
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult NoHouseHold()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult JoinHousehold() // will have to modify later 10/26/17
        {
            //Implementation for creating and joining household
            return View();
        }

        public async Task<ActionResult> LeaveHousehold()
        {
            //Implementation of leaving household
            var user = db.Users.Find(User.Identity.GetUserId());
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return View();
        }
    }
}