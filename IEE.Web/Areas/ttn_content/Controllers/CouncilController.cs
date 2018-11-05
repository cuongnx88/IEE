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
    [Authorize(Roles ="Admin")]
    public class CouncilController : BaseController
    {
        private readonly IRepository<Council> _councilRepo;
        public CouncilController() 
        {
            var unitOfWork = new UnitOfWork();
            _councilRepo = unitOfWork.GetRepository<Council>();
             
        }
        // GET: ttn_content/Council
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _councilRepo.GetMany(t => t.IsDeleted == false);

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
        public ActionResult Create()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(CouncilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var council = Mapper.Map<CouncilViewModel, Council>(model);
            council.IsDeleted = false;
            council.CreatedBy = User.UserId;
            council.ModifiedBy = User.UserId;
            council.CreatedDate = council.ModifiedDate = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/staff/"), file_name);
                    file.SaveAs(file_src);
                    break;
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            council.Photo = file_name;

            _councilRepo.InsertAndSubmit(council);
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var council = _councilRepo.Get(t => t.Id == id);
            var model = Mapper.Map<Council, CouncilViewModel>(council);
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(CouncilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var council = Mapper.Map<CouncilViewModel, Council>(model);
            council.IsDeleted       = false;
            council.ModifiedBy      = User.UserId;
            council.ModifiedDate    = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/staff/"), file_name);
                    file.SaveAs(file_src);

                    break;
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            if (!string.IsNullOrEmpty(file_name))
                council.Photo = file_name;

            _councilRepo.UpdateAndSubmit(council);
            return RedirectToAction("index");
        }

        public ActionResult Delete(int id) {
            var council = _councilRepo.Get(t => t.Id == id);
            _councilRepo.DeleteAndSubmit(council);
            return RedirectToAction("index");
        }

        public FileResult Download(string fileName)
        {
            var filepath = Path.Combine(Server.MapPath("/photo/staff/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

    }
}