using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Service.Abstract;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;
using IEE.Model;
using AutoMapper;
using System.Configuration;
using PagedList;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class CalendarController : BaseController
    {
        private readonly IRepository<Calendar> _calendarRepo;
        public CalendarController()
        {
            var unitOfWork = new UnitOfWork();
             _calendarRepo= unitOfWork.GetRepository<Calendar>();
           
        }
        // GET: ttn_content/Calendar
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _calendarRepo.GetMany(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                page = 1;
            }
            else
            {
                keyword = CurrentFilter;
            }

            ViewBag.CurrentFilter = keyword;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(t => t.Name).ThenByDescending(t => t.Id).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CalendarViewModel model, string[] selectedAppointment)
        {
            if (!ModelState.IsValid)
            {
               return View(model);
            }
            var calendar = Mapper.Map<CalendarViewModel, Calendar>(model);
            calendar.CreatedBy = User.UserId;
            calendar.ModifiedBy = User.UserId;
            calendar.CreatedDate = calendar.ModifiedDate = DateTime.Now;
            calendar.IsDeleted = false;
            if(selectedAppointment != null)
                calendar.FromTime = string.Join(",", selectedAppointment);
            _calendarRepo.InsertAndSubmit(calendar);

            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var calendar = _calendarRepo.Get(t => t.Id == id);
            List<int> appointments = new List<int>();
            if (!string.IsNullOrEmpty(calendar.FromTime))
            {
                appointments = new List<int>(Array.ConvertAll(calendar.FromTime.Split(','), int.Parse));
            }
            
            ViewBag.DaysInWeek = appointments;

            var model = Mapper.Map<Calendar, CalendarViewModel>(calendar);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(CalendarViewModel model, string[] selectedAppointment)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var calendar = Mapper.Map<CalendarViewModel, Calendar>(model);
            if (selectedAppointment != null)
                calendar.FromTime = string.Join(",", selectedAppointment);
            calendar.ModifiedDate = DateTime.Now;
            calendar.ModifiedBy = User.UserId;
            calendar.IsDeleted = false;
            _calendarRepo.UpdateAndSubmit(calendar);

            return RedirectToAction("index");
        }
        public ActionResult Delete(int id)
        {
            var calendar = _calendarRepo.Get(t => t.Id == id);
            _calendarRepo.DeleteAndSubmit(calendar);
            return RedirectToAction("index");
        }
    }
}