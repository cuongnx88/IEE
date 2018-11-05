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

namespace IEE.Web.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;

        public const int _pageSize = 21;
        // GET: Recruitment
        public LibraryController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _postRepo = unitOfWork.GetRepository<Post>();

            ViewBag.PageSize = _pageSize;
        }
        // GET: News
        public ActionResult Index(int? pageNum)
        {
            pageNum = pageNum ?? 0;
            ViewBag.IsEndOfRecords = false;
            if (Request.IsAjaxRequest())
            {
                var posts = GetRecordForPage(pageNum.Value);

                ViewBag.IsEndOfRecords = (posts.Any()) && ((pageNum.Value * _pageSize) >= posts.Last().Key);
                return PartialView("_Library", posts);
            }
            else
            {
                LoadAllPostToSession(26);
                ViewBag.Posts = GetRecordForPage(pageNum.Value);
                return View("Index");
            }
        }

        public ActionResult Content(int id)
        {
            var post = _postRepo.Get(t => t.Id == id);
            return View(post);
        }

        private Dictionary<int, Post> GetRecordForPage(int pageNum)
        {
            Dictionary<int, Post> posts = (Session["Library"] as Dictionary<int, Post>);
            int from = (pageNum * _pageSize);
            int to = from + _pageSize;
            return posts
                .Where(x => x.Key > from && x.Key <= to)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private void LoadAllPostToSession(int? categoryId)
        {
            var category = _categoryRepo.Get(t => t.Id == categoryId.Value);

            List<Post> posts = new List<Post>();
            foreach (var post in category.PostCategories.Select(p => p.Post).ToList())
            {
                bool exist = posts.Any(t => t.Id == post.Id);
                if (!exist)
                {
                  
                    posts.Add(post);
                }
            }

            int custIndex = 1;
            Session["Library"] = posts.OrderBy(t => t.OrderNumber).ThenByDescending(t => t.PublishedDate).ToDictionary(x => custIndex++, x => x);
            ViewBag.TotalNumberCustomers = posts.Count();
        }
    }
}
