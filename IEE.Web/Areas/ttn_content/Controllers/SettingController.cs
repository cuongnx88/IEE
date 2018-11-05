using IEE.Service.Abstract;
using IEE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using PagedList;
using System.Configuration;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingController : BaseController
    {
        private readonly IRepository<Setting> _settingRepo;
        public SettingController() {
            var unitOfWork = new UnitOfWork();

            _settingRepo = unitOfWork.GetRepository<Setting>();
            
        }

        // GET: ttn_content/Setting
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _settingRepo.GetMany(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Key.ToLower().Contains(keyword.ToLower()));
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
            return View(model.OrderByDescending(t => t.Id).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));

        }
        public ActionResult Edit(int id)
        {
            var setting = _settingRepo.Get(t => t.Id == id);
            var model = Mapper.Map<Setting, SettingViewModel>(setting);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(SettingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var setting = Mapper.Map<SettingViewModel, Setting>(model);
            setting.ModifiedDate    = DateTime.Now;
            setting.ModifiedBy      = User.UserId;
            setting.IsDeleted       = false;
            _settingRepo.UpdateAndSubmit(setting);

            return RedirectToAction("index");
        }
    }
}