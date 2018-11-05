using System;
using System.Collections.Generic;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using System.Configuration;
using System.Web;
using IEE.Web.Business;
using System.Net.Mail;
using System.Text;
using System.Net;
using IEE.Infrastructure;

namespace IEE.Web.Controllers
{
    public class ConsultController : Controller
    {
        private readonly IRepository<Consultant> _consultantRepo;
        private readonly IRepository<Program> _proRepo;
        public ConsultController()
        {
            UnitOfWork dbContext = new UnitOfWork();

            _consultantRepo = dbContext.GetRepository<Consultant>();
            _proRepo = dbContext.GetRepository<Program>();
        }
        public ActionResult Index()
        {
            return View();
        }
        // GET: Consultant
        public ActionResult Register()
        {
            return PartialView("_Register");
        }
        [HttpPost]
        public ActionResult Register(ConsultantViewModel model)
        {
            return View();
        }
    }
}