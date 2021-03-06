﻿using System;
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
using System.Net.Mail;
using System.Configuration;

namespace jwhitehead_FinancialPortal.Controllers
{
    [RequireHttps] // one of the steps to force the page to render secure page.
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

            ViewBag.NumberInHouse = db.Users.Where(u => u.HouseholdId == user.HouseholdId).Count();

            // Total Household Expenses
            var TotalUserExpenses = user.Household.Budgets.ToList();
            decimal spentTotal = 0;
            foreach (var item in TotalUserExpenses)
            {
                spentTotal += item.SpentAmount.Value;
            }
            ViewBag.TotalUserSpending = spentTotal;

            // Total Household Income
            var transactions = user.Household.BankAccounts.SelectMany(t => t.Transactions);
            var TotalIncome = transactions.Where(t => t.Amount > 0).ToList();
            decimal incomeTotal = 0;
            foreach (var item in TotalIncome)
            {
                incomeTotal += item.Amount;
            }
            ViewBag.TotalHouseholdIncome = incomeTotal;

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

        // GET: Households/Invite
        [AuthorizeHouseholdRequired]
        public ActionResult InviteToJoin()
        {
            return View();
        }


        // POST: Households/Invite
        [AuthorizeHouseholdRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteToJoin(EmailModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var invitee = db.Users.FirstOrDefault(u => u.Email == model.ToEmail);
                    var me = db.Users.Find(User.Identity.GetUserId());
                    model.Id = me.HouseholdId.Value;
                    if (invitee != null && invitee.HouseholdId == model.Id) // check if passed-in email from form is assigned to a household. If so, alert.
                    {
                        return RedirectToAction("UserAlreadyAssignedToHoushold");
                    }

                    var callbackUrl = "";                  
                    if (invitee != null) // check database for a match against passed-in Email name from form.
                    {
                        // invitee is already in database so send them back to a non-register page where they just confirm.
                        callbackUrl = Url.Action("JoinHousehold", "Households", new { id = model.Id }, protocol: Request.Url.Scheme);
                    }
                    else
                    {
                        // invitee email is not in database so send them back to a register page.
                        callbackUrl = Url.Action("Register", "Account", new { id = model.Id }, protocol: Request.Url.Scheme);
                    }
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "FinancialBudgeter<" + me.Email + ">";
                    var subject = "Invitation to Join Household!";
                    var to = model.ToEmail;

                    var email = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = string.Format(body, me.FullName, model.Body, "Please click on the link below to confirm invitation: <br /> <a href=\"" + callbackUrl + "\">Link to invitation.</a>"),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("InviteSent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }


        // GET: Households/InviteSent
        public ActionResult InviteSent()
        {
            return View();
        }


        // GET: Households/Invite
        public ActionResult UserAlreadyAssignedToHoushold()
        {
            return View();
        }
        

        // GET: Households/Join -- for Existing users in Database only
        public ActionResult JoinHousehold(int? id)
        {
            // pass household to view
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


        // POST: Households/Join
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JoinHousehold(int id)
        {
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseholdId = household.Id;
            db.SaveChanges();

            await HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Index"); // show household after joining.
        }


        // GET: Households/Leave
        public ActionResult LeaveHousehold()
        {
            var currentHouseholdId = User.Identity.GetHouseholdId();
            var currentHousehold = db.Households.Find(currentHouseholdId);
            return View(currentHousehold);
        }


        // POST: Households/Leave
        [HttpPost]
        public async Task<ActionResult> LeaveHousehold([Bind(Include = "Id,Name,Created,AuthorId")] Household household)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseholdId = null;
            db.SaveChanges();

            await HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Index","Home"); // Eventually the splash page.
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            // check to see if user is already part of a household
            // if so, prompt that they must leave current household before creating another one.
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null)
            {
            return View();
            }
            // part of a household, leave first before joining or creating another household
            return RedirectToAction("LeaveHousehold");
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
