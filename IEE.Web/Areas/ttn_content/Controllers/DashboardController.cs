using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class DashboardController : BaseController
    {
        // GET: ttn_content/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Menu()
        {
            return PartialView("Menu");
        }
    }
}