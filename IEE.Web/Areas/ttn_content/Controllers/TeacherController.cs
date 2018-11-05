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
    public class TeacherController : BaseController
    {
        private readonly IRepository<TeacherHighlight> _teacherRepo;
        public TeacherController()
        {
            var unitOfWork = new UnitOfWork();
            _teacherRepo = unitOfWork.GetRepository<TeacherHighlight>();
            
        }
        // GET: ttn_content/teacher
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _teacherRepo.GetMany(t => t.IsDeleted == false);

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
        public ActionResult Create(TeacherHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var teacher = Mapper.Map<TeacherHighlightViewModel, TeacherHighlight>(model);
            teacher.IsDeleted = false;
            teacher.CreatedBy = teacher.ModifiedBy = User.UserId;
            teacher.CreatedDate = teacher.ModifiedDate = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/teacher/"), file_name);
                    file.SaveAs(file_src);
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            teacher.Photo = file_name;

            _teacherRepo.InsertAndSubmit(teacher);

            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var teacher = _teacherRepo.Get(t => t.Id == id);
            var model = Mapper.Map<TeacherHighlight, TeacherHighlightViewModel>(teacher);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(TeacherHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var teacher = Mapper.Map<TeacherHighlightViewModel, TeacherHighlight>(model);
            teacher.IsDeleted = false;
            teacher.ModifiedBy = User.UserId;
            teacher.ModifiedDate = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/teacher/"), file_name);
                    file.SaveAs(file_src);
                }
            }

            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }

            if (!string.IsNullOrEmpty(file_name))
                teacher.Photo = file_name;

            _teacherRepo.UpdateAndSubmit(teacher);
            return RedirectToAction("index");
        }
        public ActionResult Delete(int id)
        {
            var teacher = _teacherRepo.Get(t => t.Id == id);
            _teacherRepo.DeleteAndSubmit(teacher);

            return RedirectToAction("index");
        }
    }
}