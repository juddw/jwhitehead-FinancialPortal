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

namespace jwhitehead_FinancialPortal.Controllers
{
    public class HouseholdsController : Universal
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
                    return RedirectToAction("Create");
                }               
                else
                {
                    return View(db.Households.Where(u => u.AuthorId == userId).ToList()); // user has a household so return the household to the view
                }
            }
            else
            {
                return View();
            }
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

        public ActionResult JoinHousehold()
        {
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
        public ActionResult Create([Bind(Include = "Id,Name,Created,AuthorId")] Household household)
        {
            if (ModelState.IsValid)
            {
                household.Created = DateTimeOffset.UtcNow; // added in View/Web.config and CustomHelpers.cs
                household.AuthorId = User.Identity.GetUserId();
                db.Households.Add(household);
                db.SaveChanges();
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
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
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
