using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jwhitehead_FinancialPortal.Controllers
{
    [RequireHttps] // one of the steps to force the page to render secure page.
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