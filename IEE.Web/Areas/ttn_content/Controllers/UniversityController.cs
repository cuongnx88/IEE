using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Service.Abstract;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using System.Configuration;
using PagedList;
using System.IO;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class UniversityController : BaseController
    {
        private readonly IRepository<UniversityHighlight> _universityRepo;
        public UniversityController()
        {
            var unitOfWork = new UnitOfWork();
            _universityRepo = unitOfWork.GetRepository<UniversityHighlight>();
            
        }
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _universityRepo.GetMany(t => t.IsDeleted == false).OrderBy(r=>r.Ranking).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Name.ToLower().Contains(keyword.ToLower())).ToList();
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
        public ActionResult Create(UniversityHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var university = Mapper.Map<UniversityHighlightViewModel, UniversityHighlight>(model);
            university.IsDeleted = false;
            university.CreatedBy = university.ModifiedBy = User.UserId;
            university.CreatedDate = university.ModifiedDate = DateTime.Now;

            //process upload an image
            string file_name = string.Empty, file_src = string.Empty;
            var allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
            bool isAllowed = true;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    
                    if (file.ContentLength > 1048576)
                    {
                        isAllowed = false;
                        break;
                    }
                    file_name = Guid.NewGuid() + Path.GetExtension(fileName);
                    file_src = Path.Combine(Server.MapPath("~/photo/university/"), file_name);
                    file.SaveAs(file_src);
                }
            }

            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }

            university.Photo = file_name;

            _universityRepo.InsertAndSubmit(university);

            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var university = _universityRepo.Get(t => t.Id == id);
            var model = Mapper.Map<UniversityHighlight, UniversityHighlightViewModel>(university);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(UniversityHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var university = Mapper.Map<UniversityHighlightViewModel, UniversityHighlight>(model);
            university.IsDeleted = false;
            university.ModifiedBy = User.UserId;
            university.ModifiedDate = DateTime.Now;

            //process upload an image
            string file_name = string.Empty, file_src = string.Empty;
            var allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
            bool isAllowed = true;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    
                    if (file.ContentLength > 1048576)
                    {
                        isAllowed = false;
                        break;
                    }
                    file_name = Guid.NewGuid() + Path.GetExtension(fileName);
                    file_src = Path.Combine(Server.MapPath("~/photo/university/"), file_name);
                    file.SaveAs(file_src);
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            if (!string.IsNullOrEmpty(file_name))
                university.Photo = file_name;

            _universityRepo.UpdateAndSubmit(university);
            return RedirectToAction("index");
        }
        public ActionResult Delete(int id)
        {
            var university = _universityRepo.Get(t => t.Id == id);
            _universityRepo.DeleteAndSubmit(university);

            return RedirectToAction("index");
        }
    }
}