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
    public class StudentController : BaseController
    {
        private readonly IRepository<StudentHighlight> _studentRepo;
        public StudentController()
        {

            var unitOfWork = new UnitOfWork();
            _studentRepo = unitOfWork.GetRepository<StudentHighlight>();
            
        }
        // GET: ttn_content/Student
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _studentRepo.GetMany(t => t.IsDeleted == false);

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
            ViewBag.Programs = this.Programs();
            return View();
        }
        [HttpPost]
        public ActionResult Create(StudentHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = this.Programs();
                return View(model);
            }
            var student = Mapper.Map<StudentHighlightViewModel, StudentHighlight>(model);
            student.IsDeleted = false;
            student.CreatedBy = student.ModifiedBy = User.UserId;
            student.CreatedDate = student.ModifiedDate = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/student/"), file_name);
                    file.SaveAs(file_src);
                }
            }
            if (!isAllowed)
            {
                ViewBag.Programs = this.Programs();
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }

            student.Photo = file_name;
            _studentRepo.InsertAndSubmit(student);

            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Programs = this.Programs();
            var student = _studentRepo.Get(t => t.Id == id);
            var model = Mapper.Map<StudentHighlight, StudentHighlightViewModel>(student);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(StudentHighlightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = this.Programs();
                return View(model);
            }

            var student = Mapper.Map<StudentHighlightViewModel, StudentHighlight>(model);
            student.IsDeleted       = false;
            student.ModifiedBy      = User.UserId;
            student.ModifiedDate    = DateTime.Now;

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
                    file_src = Path.Combine(Server.MapPath("~/photo/student/"), file_name);
                    file.SaveAs(file_src);
                }
            }

            if (!isAllowed)
            {
                ViewBag.Programs = this.Programs();
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }

            if (!string.IsNullOrEmpty(file_name))
                student.Photo = file_name;

            _studentRepo.UpdateAndSubmit(student);
            return RedirectToAction("index");
        }
        public ActionResult Delete(int id)
        {
            var student = _studentRepo.Get(t => t.Id == id);
            _studentRepo.DeleteAndSubmit(student);

            return RedirectToAction("index");
        }

    }
}