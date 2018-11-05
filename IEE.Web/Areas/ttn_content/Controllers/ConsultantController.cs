using System;
using System.Linq;
using System.Web.Mvc;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;
using AutoMapper;
using System.Configuration;
using PagedList;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConsultantController : BaseController
    {
        private readonly IRepository<Consultant> _consultantRepository;
        public ConsultantController()
        {
            var unitOfWork = new UnitOfWork();
            _consultantRepository = unitOfWork.GetRepository<Consultant>();
             
        }
        // GET: ttn_content/Consultant
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _consultantRepository.GetMany(t => t.IsDeleted == false);

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
            return View(model.OrderByDescending(t => t.Id).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));

        }

        public ActionResult Detail(int id)
        {
            var consultant = _consultantRepository.Get(t => t.Id == id);
            var model = Mapper.Map<Consultant, ConsultantViewModel>(consultant);
            return View(model);
        }
        [HttpPost]
        public ActionResult Detail(ConsultantViewModel model)
        {
            try
            {
                var consultant = Mapper.Map<ConsultantViewModel, Consultant>(model);
                consultant.ModifiedBy   = User.UserId;
                consultant.ModifiedDate = DateTime.Now;
                consultant.IsDeleted    = false;
                _consultantRepository.UpdateAndSubmit(consultant);
                return RedirectToAction("index");
            }
            catch
            {
                throw new Exception();
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var consultant          = _consultantRepository.Get(t => t.Id == id);
                _consultantRepository.DeleteAndSubmit(consultant);
                return RedirectToAction("index");
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}