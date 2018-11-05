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
    public class CalendarsController : Controller
    {
        private readonly IRepository<Calendar> _calendarRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Setting> _settingRepo;
        public CalendarsController()
        {
            var unitOfWork = new UnitOfWork();
            _calendarRepo = unitOfWork.GetRepository<Calendar>();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _settingRepo = unitOfWork.GetRepository<Setting>();
        }
        // GET: Calendar
        public ActionResult Index()
        {
            var categories = _categoryRepo.GetMany(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;
            ViewBag.Facebook = _settingRepo.Get(t => t.Id == 2).Value;
            ViewBag.Youtube = _settingRepo.Get(t => t.Id == 3).Value;
            ViewBag.Linked = _settingRepo.Get(t => t.Id == 6).Value;
            ViewBag.Gplus = _settingRepo.Get(t => t.Id == 5).Value;
            ViewBag.Instagram = _settingRepo.Get(t => t.Id == 4).Value;

            ViewBag.Hotline = _settingRepo.Get(t => t.Id == 1).Value;
            ViewBag.Address = _settingRepo.Get(t => t.Key == "address").Value;
            ViewBag.Phone = _settingRepo.Get(t => t.Key.Equals("phone")).Value;

            return View();
        }

        
        public JsonResult Events()
        {
            var calendars = _calendarRepo.GetMany(t => t.IsDeleted == false).OrderByDescending(t=>t.FromDate);
            List<CalendarArray> rows = new List<CalendarArray>();
            CalendarArray row = null;
            foreach (var calendar in calendars)
            {
                TimeSpan ts = calendar.ToDate.Value - calendar.FromDate.Value;
                int day = ts.Days;
                if(day == 0)
                {
                    var date = calendar.FromDate.Value;
                    row = new CalendarArray();
                    row.title = calendar.Name;
                    row.start = date.ToString("yyyy-MM-dd");
                    rows.Add(row);
                }
                else
                {
                    for (int i = 0; i <= day; i++)
                    {
                        var date = calendar.FromDate.Value.AddDays(i);
                        int dayinweek = (int)date.DayOfWeek;
                        if (calendar.FromTime.Contains(dayinweek.ToString()))
                        {
                            row = new CalendarArray();
                            row.title = calendar.Name;
                            row.start = date.ToString("yyyy-MM-dd");
                            rows.Add(row);
                        }

                    }
                }
                
            }
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        public class CalendarArray { 
            public string title { get; set; }
            public string start { get; set; }
        }
    }

}