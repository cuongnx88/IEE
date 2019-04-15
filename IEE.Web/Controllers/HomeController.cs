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
using IEE.Web.Models;
using System.Data.Entity;
using IEE.Web.Areas.ttn_content.Models;

namespace IEE.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Setting> _settingRepo;
        private readonly IRepository<Post> _postRepo;
        private readonly IRepository<Testimonial> _testimonialRepo;
        private readonly IRepository<Banner> _bannerRepo;
        private readonly IRepository<UniversityHighlight> _universityHighlight;
        private readonly IRepository<TeacherHighlight> _teacherHighlight;
        private readonly IRepository<StudentHighlight> _studentHighlight;
        UnitOfWork dbContext = new UnitOfWork();
        private SATEntities db = new SATEntities();
        public HomeController()
        {



            _userRepo = dbContext.GetRepository<User>();
            _categoryRepo = dbContext.GetRepository<Category>();
            _settingRepo = dbContext.GetRepository<Setting>();
            _postRepo = dbContext.GetRepository<Post>();
            _testimonialRepo = dbContext.GetRepository<Testimonial>();
            _bannerRepo = dbContext.GetRepository<Banner>();
            _universityHighlight = dbContext.GetRepository<UniversityHighlight>();
            _teacherHighlight = dbContext.GetRepository<TeacherHighlight>();
            _studentHighlight = dbContext.GetRepository<StudentHighlight>();
        }

        public ActionResult Index()
        {
            var listLevel = CommonConstants.GetListLevel();

            /*BANNER
            --------------------------------------------*/
            var banners = _bannerRepo.GetMany(t => t.IsDeleted == false && t.IsLock == true && t.IsHeader == false).OrderBy(banner => banner.BannerIndex).ToList();
            ViewBag.Banners = banners;
            /*
            --------------------------------------------*/
            var testimonials = _testimonialRepo.GetMany(t => t.IsDeleted == false).ToList();
            ViewBag.Testimonials = testimonials;
            /*HEADER MENU
            --------------------------------------------*/
            var categories = _categoryRepo.GetMany(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;

            var ieltCat = _categoryRepo.GetMany(c => c.Name.Contains("IELT") && c.IsDeleted == false).Where(c => c.Name == "IELTS Foundation" || c.Name == "IELTS 5.5" || c.Name == "IELTS 6.5" || c.Name == "IELTS >7.5").OrderBy(c => c.OrderNumber).ToList();
            var satCat = _categoryRepo.GetMany(c => c.Name.Contains("SAT") && c.IsDeleted == false).Where(c => c.Name == "SAT Beginner" || c.Name == "SAT Intermediate" || c.Name == "SAT Advanced" || c.Name == "SAT Super Advanced").OrderBy(c => c.OrderNumber).ToList();
            if (ieltCat != null)
            {
                ieltCat.AddRange(satCat);
                ViewBag.ListCat = ieltCat;
            }



            /*TOP NEWS
            --------------------------------------------*/
            var usAbroad = db.Categories.FirstOrDefault(t => t.Id == 7);
            var handbook = db.Categories.FirstOrDefault(t => t.Id == 6);
            var ieenews = db.Categories.FirstOrDefault(t => t.Id == 8);
            var hostnew = db.Categories.FirstOrDefault(t => t.Id == 19);

            ViewBag.UsAbord = usAbroad;
            ViewBag.Handbook = handbook;
            ViewBag.IeeNews = ieenews;
            /*TOP NEWS
            --------------------------------------------*/
            IEnumerable<Post> posts = null;
            if (hostnew != null)
            {
               
                    // hostnew.PostCategories = db.PostCategories.Where(p => p.PostID == hostnew.Id).ToList();
                    if (hostnew.PostCategories.Select(p => p.Post).Count() > 0)
                    {
                        posts = hostnew.PostCategories.Select(p => p.Post).Take(2).ToList();
                    }

                

                //_postRepo.GetMany(t => t.Categories.Where(c => t.Id == hostnew.Id).Any() == true).Take(2);
            }

            ViewBag.Top = posts;

            /*Khai giang
            --------------------------------------------*/
            var openingPost = (from p in dbContext.DataContext.Posts
                               join pc in dbContext.DataContext.PostCategories
                               on p.Id equals pc.PostID
                               join c in dbContext.DataContext.Categories
                               on pc.CategoryID equals c.Id
                               where c.Name == "Thông tin khai giảng" &&c.Position!=null &&c.Position.Contains("Home")
                               select p
                             ).OrderByDescending(a=>a.Id).Take(4).ToList();
            ViewBag.OpeningPostCat = "Thông tin khai giảng";
            ViewBag.OpeningPost = openingPost;

            /*blog du hoc
           --------------------------------------------*/
            var blog = (from p in dbContext.DataContext.Posts
                        join pc in dbContext.DataContext.PostCategories
                        on p.Id equals pc.PostID
                        join c in dbContext.DataContext.Categories
                        on pc.CategoryID equals c.Id
                        where c.Name.Contains("Blog") && c.Position != null && c.Position.Contains("Home")
                        select p
                             ).Take(4).ToList();

            ViewBag.Blog = blog;

            /*Hoạt động IEE
           --------------------------------------------*/
            var ieeActivity = (from p in dbContext.DataContext.Posts
                        join pc in dbContext.DataContext.PostCategories
                        on p.Id equals pc.PostID
                        join c in dbContext.DataContext.Categories
                        on pc.CategoryID equals c.Id
                        where c.Name.Contains("Hoạt động IEE") && c.Position != null && c.Position.Contains("Home")
                               select p
                             ).Take(3).ToList();

            ViewBag.IeeActivityCat = "Hoạt động IEE".ToSeoUrl();
            ViewBag.IeeActivityPost = ieeActivity;

            /*other homepage cate
           --------------------------------------------*/
            var listPartialViewModel = new List<PartialViewModel>();
            var others = dbContext.DataContext.Categories.Where(c => c.IsSystem.HasValue && !c.IsSystem.Value && c.Position != null && c.Position.Contains("Home")).ToList();
            foreach (var item in others)
            {
                var partialViewModel = new PartialViewModel();
                partialViewModel.Cate = item;
                if (item.PostCategories.Where(c => c.CategoryID == item.Id)!=null&& item.PostCategories.Where(c => c.CategoryID == item.Id).Count()>0)
                {
                    var list = item.PostCategories.Where(c => c.CategoryID == item.Id).Select(p => p.Post).OrderByDescending(p=>p.CreatedDate).ToList();
                    if (item.DisplayType=="List")
                    {
                        list = list.Take(3).ToList();
                    }
                    else if (item.DisplayType == "Grid")
                    {
                        list = list.Take(6).ToList();
                    }
                    else if (item.DisplayType == "Blog")
                    {
                        list = list.Take(4).ToList();
                    }
                    else if (item.DisplayType=="Content")
                    {
                        list = list.Take(1).ToList();
                    }
                    partialViewModel.ListPost = list;
                }
                listPartialViewModel.Add(partialViewModel);
            }
            ViewBag.ListPartialViewModel = listPartialViewModel;
            /*
            --------------------------------------------*/

            //var calendar = dbContext.GetRepository<Calendar>().GetMany(x => DbFunctions.DiffDays(x.CreatedDate, DateTime.Now) >=0 ).OrderBy(c => c.CreatedDate).Take(3).ToList();
            //ViewBag.Calendar = calendar;

            var files = dbContext.GetRepository<Medium>().GetMany(m => m.Type ==2).OrderByDescending(f=>f.CreatedDate).ToList();//uploaded document
            ViewBag.Files = files;

            var topUniversity = _universityHighlight.GetTop(6).OrderBy(r=>r.Ranking).ToList();
            ViewBag.University = topUniversity;

            var topTeacher = _teacherHighlight.GetAll().ToList();
            ViewBag.Teacher = topTeacher;

            var allStudent = _studentHighlight.GetAll().ToList();
            var topSat = allStudent.Where(t => t.SAT != null).OrderByDescending(s => s.SAT).Take(6).ToList();
            ViewBag.TopSAT = topSat;

            var topIELT = allStudent.Where(t=>t.IELTS!=null).OrderByDescending(s => s.IELTS).Take(6).ToList();
            ViewBag.TopIELT = topIELT;

            ViewBag.Facebook = _settingRepo.Get(t => t.Id == 2).Value;
            ViewBag.Youtube = _settingRepo.Get(t => t.Id == 3).Value;
            ViewBag.Linked = _settingRepo.Get(t => t.Id == 6).Value;
            ViewBag.Gplus = _settingRepo.Get(t => t.Id == 5).Value;
            ViewBag.Instagram = _settingRepo.Get(t => t.Id == 4).Value;

            ViewBag.Hotline = _settingRepo.Get(t => t.Id == 1).Value;
            ViewBag.Address = _settingRepo.Get(t => t.Key == "address").Value;
            ViewBag.Phone = _settingRepo.Get(t => t.Key.Equals("phone")).Value;

            ViewBag.Title = db.Settings.FirstOrDefault(t => t.Key.Equals("title")).Value;
            ViewBag.MetaKeys = db.Settings.FirstOrDefault(t => t.Key.Equals("meta_key")).Value;
            ViewBag.MetaDescription = db.Settings.FirstOrDefault(t => t.Key.Equals("meta_description")).Value;

            return View();
        }
        public void SetInforHomepage(SATEntities db)
        {
            var categories = db.Categories.Where(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;
            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}