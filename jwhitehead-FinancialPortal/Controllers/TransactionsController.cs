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
    [RequireHttps] // one of the steps to force the page to render secure page.
    [AuthorizeHouseholdRequired]
    public class TransactionsController : Universal
    {
        // GET: Transactions
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Transactions.ToList());
            }

            // view transactions for just the logged in user's accounts.
            var user = db.Users.Find(User.Identity.GetUserId());
            var transactions = user.Household.BankAccounts.SelectMany(t => t.Transactions);
            return View(transactions.OrderByDescending(t => t.TransactionDate).ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            // filter and show only accounts pertaining to current user.
            var user = db.Users.Find(User.Identity.GetUserId());
            var userOnlyBankAccounts = user.Household.BankAccounts.ToList();

            ViewBag.BankAccountId = new SelectList(userOnlyBankAccounts, "Id", "BankAccountName");
            ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UpdatedBy,Reconciled,Void,TransactionTypeId,Amount,Description,CategoryId,BankAccountId,TransactionDate,ReconciliationDate")] Transaction transaction)
        {

            if (ModelState.IsValid)
            {
                transaction.TransactionDate = DateTimeOffset.UtcNow; // added in View/Web.config and TimeZoneHelpers.cs
                db.Transactions.Add(transaction);

                BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
                // Add/Subtract from Account Balance
                // check type: 1, debit. 2, credit.
                if (transaction.TransactionTypeId == 1)
                {
                    // always make number negative if debit
                    if (transaction.Amount > 0)
                    {
                        transaction.Amount *= -1;
                    }
                    account.Balance += transaction.Amount;
                }
                else if (transaction.TransactionTypeId == 2)
                {
                    // always make number positive if credit
                    if (transaction.Amount < 0)
                    {
                        transaction.Amount *= -1;
                    }
                    account.Balance += transaction.Amount;
                }

                // check for account overdraft on this account's transaction.
                if (account.Balance < 0)
                {
                    ViewBag.Overdraft = "True";
                }
                else
                {
                    ViewBag.Overdraft = "False";
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountName", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            return View(transaction);
        }


        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            // filter and show only accounts pertaining to current user.
            var user = db.Users.Find(User.Identity.GetUserId());
            var userOnlyBankAccounts = user.Household.BankAccounts.ToList();

            //// filter and show only users in household.
            //// find all users whose HouseholdId == Household.Id
            //var usersInHousehold = db.Users.Where(u => u.HouseholdId == transaction.BankAccount.Household.Id).ToList();

            ViewBag.BankAccountId = new SelectList(userOnlyBankAccounts, "Id", "BankAccountName", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(c => c.Name), "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            //ViewBag.HouseHoldUsers = new SelectList(usersInHousehold, "Id", "Name", transaction.BankAccount.Household.Id);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UpdatedBy,Reconciled,Void,TransactionTypeId,Amount,Description,CategoryId,BankAccountId,TransactionDate,ReconciliationDate")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);

                // Check what the amount was before user edited the amount.
                var oldAmount = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id).Amount;
     
                // Undo the transaction amount previously done on the account before applying new amount entered.
                account.Balance += oldAmount * -1;

                // Add/Subtract from Account Balance
                // check type: 1, debit. 2, credit.
                if (transaction.TransactionTypeId == 1)
                {
                    // always make number negative if debit
                    if (transaction.Amount > 0)
                    {
                        transaction.Amount *= -1;
                    }
                    account.Balance += transaction.Amount;
                }
                else if (transaction.TransactionTypeId == 2)
                {
                    // always make number positive if credit
                    if (transaction.Amount < 0)
                    {
                        transaction.Amount *= -1;
                    }
                    account.Balance += transaction.Amount;
                }

                // check for account overdraft on this account's transaction.
                if (account.Balance < 0)
                {
                    ViewBag.Overdraft = "True";
                }
                else
                {
                    ViewBag.Overdraft = "False";
                }

                if (transaction.ReconciliationDate != null)
                {
                    transaction.Reconciled = true;
                }

                //db.Entry(account).State = EntityState.Modified;

                //transactionOldValue.Amount = transaction.Amount;
                db.Entry(transaction).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountName", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            //ViewBag.HouseHoldUsers = new SelectList(db.Households, "Id", "HouseholdId", transaction.BankAccount.Household.Users);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
            // REVERSE the transaction
            // check type: 1, debit. 2, credit.
            if (transaction.TransactionTypeId == 1)
            {
                account.Balance += transaction.Amount;
            }
            else
            {
                account.Balance -= transaction.Amount;
            }

            // check for account overdraft on this account's transaction.
            if (account.Balance < 0)
            {
                ViewBag.Overdraft = "True";
            }
            else
            {
                ViewBag.Overdraft = "False";
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Transactions/Void/5
        public ActionResult Void(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            if (transaction.Void == true)
            {
                return RedirectToAction("Unvoid");
            }

            return View(transaction);
        }

        // POST: Transactions/Void/5
        [HttpPost, ActionName("Void")]
        [ValidateAntiForgeryToken]
        public ActionResult VoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
            // REVERSE the transaction but DO NOT REMOVE from database
            // check type: 1, debit. 2, credit.
            if (transaction.TransactionTypeId == 1)
            {
                account.Balance += transaction.Amount;
            }
            else
            {
                account.Balance -= transaction.Amount;
            }

            // check for account overdraft on this account's transaction.
            if (account.Balance < 0)
            {
                ViewBag.Overdraft = "True";
            }
            else
            {
                ViewBag.Overdraft = "False";
            }

            transaction.Void = true;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Transactions/Unvoid/5
        public ActionResult Unvoid(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Unvoid/5
        [HttpPost, ActionName("Unvoid")]
        [ValidateAntiForgeryToken]
        public ActionResult UnvoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
            // ADD BACK the transaction but DO NOT REMOVE from database
            // check type: 1, debit. 2, credit.
            if (transaction.TransactionTypeId == 1)
            {
                account.Balance -= transaction.Amount;
            }
            else
            {
                account.Balance += transaction.Amount;
            }

            // check for account overdraft on this account's transaction.
            if (account.Balance < 0)
            {
                ViewBag.Overdraft = "True";
            }
            else
            {
                ViewBag.Overdraft = "False";
            }

            transaction.Void = false;

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
