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
using IEE.Infrastructure;

namespace IEE.Web.Controllers
{
    public class EventController : Controller
    {
        private readonly IRepository<Banner> _bannerRepo;
        public EventController()
        {
            UnitOfWork dbContext = new UnitOfWork();

            _bannerRepo = dbContext.GetRepository<Banner>();
           
        }
        // GET: Event
        public ActionResult Index(int id)
        {
            var banner = _bannerRepo.Get(t => t.Id == id);
            return View(banner);
        }

        public ActionResult Content(int id)
        {
            return View();
        }
    }
}