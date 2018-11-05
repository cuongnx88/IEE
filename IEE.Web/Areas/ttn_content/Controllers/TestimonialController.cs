using System;
using System.Collections.Generic;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using PagedList;
using System.Configuration;
using System.Web;
using System.IO;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles ="Admin")]
    public class TestimonialController : BaseController
    {
        private readonly IRepository<Testimonial> _testimonalRepo; private SATEntities db = new SATEntities();
        public TestimonialController()
        {
            var unitOfWork = new UnitOfWork();
            _testimonalRepo = unitOfWork.GetRepository<Testimonial>();
           
        }
        // GET: ttn_content/Testimonial
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _testimonalRepo.GetMany(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Title.ToLower().Contains(keyword.ToLower())).OrderByDescending(t => t.CreatedDate);
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
        [HttpPost]
        public ActionResult Create(TestimonialViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var testimonial = Mapper.Map<TestimonialViewModel, Testimonial>(model);

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
                    file_src = Path.Combine(Server.MapPath("~/photo/testimonial/"), file_name);
                    file.SaveAs(file_src);

                    break;
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            testimonial.Photo           = file_name;
            testimonial.CreatedBy       = User.UserId;
            testimonial.ModifiedBy      = User.UserId;
            testimonial.CreatedDate     = DateTime.Now;
            testimonial.ModifiedDate    = DateTime.Now;
            testimonial.IsDeleted       = false;
            _testimonalRepo.InsertAndSubmit(testimonial);

            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var testimonial = _testimonalRepo.Get(t => t.Id == id);
            var model = Mapper.Map<Testimonial, TestimonialViewModel>(testimonial);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(TestimonialViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var testimonial = Mapper.Map<TestimonialViewModel, Testimonial>(model);
            testimonial.ModifiedBy      = User.UserId;
            testimonial.ModifiedDate    = DateTime.Now;
            testimonial.IsDeleted       = false;
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
                    file_src = Path.Combine(Server.MapPath("~/photo/testimonial/"), file_name);
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
                testimonial.Photo = file_name;

            _testimonalRepo.UpdateAndSubmit(testimonial);
            return RedirectToAction("index");
        }
        public ActionResult Delete(int id)
        {
            var testimonial = _testimonalRepo.Get(t => t.Id == id);
            _testimonalRepo.DeleteAndSubmit(testimonial);
            return RedirectToAction("index");
        }
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var testimonial = _testimonalRepo.Get(t => t.Id == id);
            var banners = db.Banners.Where(t => t.IsDeleted == false && t.IsLock == true && t.IsHeader == false).OrderBy(banner => banner.BannerIndex).ToList();
            ViewBag.Banners = banners;
            return View("~/Areas/ttn_content/Views/Testimonial/Details.cshtml",testimonial);
        }
        [AllowAnonymous]
        public ActionResult List(int? trang)
        {
            var banners = db.Banners.Where(t => t.IsDeleted == false && t.IsLock == true && t.IsHeader == false).OrderBy(banner => banner.BannerIndex).ToList();
            ViewBag.Banners = banners;
            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;
            var listTestimonial = db.Testimonials.Where(t => t.IsDeleted==false).ToList();
          
            trang = trang == null ? 0 : trang;

            var pageCount = 10;
            pageCount = 8;
            ViewBag.Trang = trang.Value;
            ViewBag.PageCount = pageCount;
            ViewBag.TotalPage = listTestimonial.Count() / pageCount;
            return View("~/Areas/ttn_content/Views/Testimonial/List.cshtml",listTestimonial);
        }
    }
}