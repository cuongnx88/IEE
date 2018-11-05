using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ttn_content/Error
        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult InternalServer()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }



    }
}