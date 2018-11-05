using System;
using System.Collections.Generic;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using IEE.Model;
using AutoMapper;
using System.Configuration;
using System.Web;
using IEE.Web.Business;
using IEE.Infrastructure;
using IEE.Web.Areas.ttn_content.Models;

namespace IEE.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;
        private SATEntities db = new SATEntities();
        public const int _pageSize = 21;
        public NewsController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _postRepo = unitOfWork.GetRepository<Post>();

            ViewBag.PageSize = _pageSize;
        }
        // GET: News
        public ActionResult Index(int? trang)
        {
            //pageNum = pageNum ?? 0;
            //ViewBag.IsEndOfRecords = false;
            //if (Request.IsAjaxRequest())
            //{
            //    var posts = GetRecordForPage(pageNum.Value);

            //    ViewBag.IsEndOfRecords = (posts.Any()) && ((pageNum.Value * _pageSize) >= posts.Last().Key);
            //    return PartialView("_Articles", posts);
            //}
            //else
            //{
            //    LoadAllPostToSession(null);
            //    ViewBag.Posts = GetRecordForPage(pageNum.Value);
            //    return View("Index");
            //}
            trang = trang ?? 0;
            var pageCount = 10;
            var model = _postRepo.GetPage(trang.Value, pageCount, o => o.ViewCount, false).ToList();

            ViewBag.TotalPage = pageCount;

            return View("Index", model);

        }

        public ActionResult Content(int id)
        {
            var categories = _categoryRepo.GetMany(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;
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
            var unitOfWork = new UnitOfWork();
            var post = _postRepo.Get(t => t.Id == id && t.Status == true);
            var catID = post.PostCategories.Where(p => p.PostID == id).Select(c => c.CategoryID).FirstOrDefault();
            var relatePost = (from p in unitOfWork.DataContext.Posts
                              join pc in unitOfWork.DataContext.PostCategories
                              on p.Id equals pc.PostID
                              join c in unitOfWork.DataContext.Categories
                              on pc.CategoryID equals c.Id
                              where c.Id == catID && p.Id!=id
                              orderby  p.CreatedDate descending
                              select p).Take(5).ToList();
            ViewBag.RelatePost = relatePost;
            return View(post);
        }
        public ActionResult Preview(int id)
        {
            var post = _postRepo.Get(t => t.Id == id);
            return View(post);
        }
        public ActionResult Category(string category, int? trang)
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
            var listCat = _categoryRepo.GetAll();
            var catModel = new Category();
            trang = trang == null ? 0 : trang;

            foreach (var item in listCat)
            {
                if (item.Name.ToSeoUrl() == category)
                {
                    catModel = item;
                    break;
                }
            }
            var pageCount = 10;
            var categories = _categoryRepo.GetMany(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;
            pageCount = 8;
            ViewBag.CategoryId = catModel.Id;
            ViewBag.Trang = trang.Value;
            ViewBag.PageCount = pageCount;
            ViewBag.TotalPage = catModel.PostCategories.Select(p => p.Post).Count() / pageCount;
            ViewBag.Cat = category;
           
            //    var partialViewModel = new PartialViewModel();
            //    partialViewModel.Cate = catModel;
            //    if (catModel.PostCategories.Where(c => c.CategoryID == catModel.Id) != null && catModel.PostCategories.Where(c => c.CategoryID == catModel.Id).Count() > 0)
            //    {
            //        var list = catModel.PostCategories.Where(c => c.CategoryID == catModel.Id).Select(p => p.Post).OrderByDescending(p => p.CreatedDate).ToList();
            //         if (catModel.DisplayType == "Content")
            //        {
            //            list = list.Take(1).ToList();
            //        ViewBag.TotalPage = 1;
            //        }
            //        partialViewModel.ListPost = list;
            //    }
               
            
            //ViewBag.PartialViewModel = partialViewModel;

            return View(catModel);
        }

        private Dictionary<int, Post> GetRecordForPage(int pageNum)
        {
            Dictionary<int, Post> posts = (Session["Posts"] as Dictionary<int, Post>);
            int from = (pageNum * _pageSize);
            int to = from + _pageSize;
            return posts
                .Where(x => x.Key > from && x.Key <= to)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private void LoadAllPostToSession(int? categoryId)
        {
            IEnumerable<Category> categories;
            if (categoryId.HasValue)
            {
                categories = _categoryRepo.GetMany(t => t.Id == categoryId.Value);
            }
            else
            {
                categories = _categoryRepo.GetMany(t => t.Controller.Equals("news"));
            }

            List<Post> posts = new List<Post>();
            foreach (var category in categories)
            {
                if (category.PostCategories.Select(p => p.Post).Count() > 0)
                {
                    var listPost = category.PostCategories.Select(p => p.Post).Where(p => p != null).ToList();


                    foreach (var post in listPost)
                    {
                        bool exist = posts.Any(t => t.Id == post.Id);
                        if (!exist)
                        {
                            posts.Add(post);
                        }
                    }
                }
            }

            int custIndex = 1;
            Session["Posts"] = posts.Where(t => t.Status != null && t.Status.Value).OrderBy(t => t.OrderNumber).ThenByDescending(t => t.PublishedDate).ToDictionary(x => custIndex++, x => x);
            ViewBag.TotalNumberCustomers = posts.Count();
        }
    }
}