using jwhitehead_FinancialPortal.Models.Helpers;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace jwhitehead_FinancialPortal.Controllers
{
    [RequireHttps] // one of the steps to force the page to render secure page.
    [Authorize]
    public class HomeController : Universal
    {
        // GET: Assigned Projects
        private HouseholdAssignHelper helper = new HouseholdAssignHelper();

        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(user.Household);
        }

        [AllowAnonymous]
        public ActionResult LandingPage()
        {
            return View();
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