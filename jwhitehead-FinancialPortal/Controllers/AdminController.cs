using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jwhitehead_FinancialPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Universal
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }
    }
}