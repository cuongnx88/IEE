using System;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using IEE.Model;
using AutoMapper;
using PagedList;
using System.Configuration;
using System.IO;
using System.Web;
using IEE.Infrastructure;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class BannerController : BaseController
    {
        private readonly IRepository<Banner> _bannerRepository;
        public BannerController()
        {
            var unitOfWork = new UnitOfWork();
            _bannerRepository = unitOfWork.GetRepository<Banner>();
          
        }
        // GET: ttn_content/Banner
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _bannerRepository.GetMany(t => t.IsDeleted == false);

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
            return View(model.OrderByDescending(t=>t.Id).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));
        }
        public ActionResult Create()
        {
            var viewModel = new BannerViewModel();
            return View(viewModel);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(BannerViewModel model,FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Position = collection.Get("BannerPosition");
            var banner = Mapper.Map<BannerViewModel, Banner>(model);

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
                    file_src = Path.Combine(Server.MapPath("~/photo/banner/"), file_name);
                    file.SaveAs(file_src);
                    break;
                }
            }
            if (!isAllowed)
            {
                ModelState.AddModelError("", "Bạn chỉ được phép upload file .png và .jpg, hoặc file của bạn có dung lượng nhỏ hơn 2Mb");
                return View(model);
            }
            banner.Photo        = file_name;
            banner.CreatedBy    = User.UserId;
            banner.ModifiedBy   = User.UserId;
            banner.CreatedDate  = DateTime.Now;
            banner.ModifiedDate = DateTime.Now;
            banner.IsDeleted = false;
            _bannerRepository.InsertAndSubmit(banner);

            return RedirectToAction("index");
            
        }

        public ActionResult Edit(int id)
        {
            var banner  = _bannerRepository.Get(t => t.Id == id);
            var model   = Mapper.Map<Banner, BannerViewModel>(banner);

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost]
        
        public ActionResult Edit(BannerViewModel model,FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var banner = Mapper.Map<BannerViewModel, Banner>(model);

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
                        file_src = Path.Combine(Server.MapPath("~/photo/banner/"), file_name);
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
                    banner.Photo = file_name;
                banner.ModifiedBy   = User.UserId;
                banner.ModifiedDate = DateTime.Now;
                banner.IsDeleted = false;
                banner.Position = collection.Get("BannerPosition");
                _bannerRepository.UpdateAndSubmit(banner);

                return RedirectToAction("index");
            }
            catch
            {
                throw new Exception();
            }
        }

        public ActionResult Delete(int id)
        {
            var banner = _bannerRepository.Get(t => t.Id == id);
            _bannerRepository.DeleteAndSubmit(banner);
            return RedirectToAction("index");
        }

        public FileResult Download(string fileName)
        {
            var filepath = Path.Combine(Server.MapPath("/photo/banner/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }
        
    }
}