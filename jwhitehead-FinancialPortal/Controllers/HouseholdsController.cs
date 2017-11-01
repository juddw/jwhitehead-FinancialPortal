using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jwhitehead_FinancialPortal.Models;
using jwhitehead_FinancialPortal.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using jwhitehead_FinancialPortal.Models.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace jwhitehead_FinancialPortal.Controllers
{
    [Authorize]
    public class HouseholdsController : Universal
    {
        // email
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Assigned Projects
        private HouseholdAssignHelper helper = new HouseholdAssignHelper();

        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(user.Household);
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        public ActionResult AskCreateJoinHousehold()
        {
            return View();
        }

        // GET: Households/Invite
        public ActionResult InviteToJoin()
        {
            return View();
        }

        // POST: Households/Invite
        [HttpPost]
        public async Task<ActionResult> InviteToJoin([Bind(Include = "Id,Email")] InviteToJoinViewModel household)
        {
            var myhousehold = db.Households.Find(household.Id);

                 // CODE FOR EMAIL NOTIFICATON
                var callbackUrl = Url.Action("ConfirmJoin", "Households", new { id = household.Id }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(household.Email, "Join Household!", "You are invited to join: " + myhousehold.Name + "!<br/><br/>  Please click <a href=\"" + callbackUrl + "\">here</a> to view invitation.");
         
            return RedirectToAction("Index");
        }

        // GET: Households/Join
        public ActionResult JoinHousehold()
        {
            return View();
        }

        // POST: Households/Join
        [HttpPost]
        public async Task<ActionResult> JoinHousehold([Bind(Include = "Id,Name,Created,AuthorId")] Household household)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseholdId = household.Id;
            db.SaveChanges();

            await HttpContext.RefreshAuthentication(user);
            return View();
        }

        // GET: Households/Leave
        public ActionResult LeaveHousehold()
        {
            return View();
        }

        // POST: Households/Leave
        [HttpPost]
        public async Task<ActionResult> LeaveHousehold([Bind(Include = "Id,Name,Created,AuthorId")] Household household)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseholdId = null;
            db.SaveChanges();

            await HttpContext.RefreshAuthentication(user);
            return View();
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Created,AuthorId")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTimeOffset.UtcNow; // added in View/Web.config and CustomHelpers.cs
                household.AuthorId = User.Identity.GetUserId();

                db.Households.Add(household);

                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                db.SaveChanges();

                await HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Index");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AuthorId,Created")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) // check that a id was passed
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null) // check for household in table
            {
                return HttpNotFound();
            }

            return View(household); // no users in household so you can delete.
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
