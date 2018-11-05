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
    public class FindController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;
        public FindController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _postRepo = unitOfWork.GetRepository<Post>();
        }
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)||string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index", "Home");
            }
            var categories = _categoryRepo.GetMany(t => t.Controller.Equals("news"));
            List<Post> posts = new List<Post>();
            foreach (var category in categories)
            {
                //var cPosts = category.Posts.Where(t => t.Title.ToLower().Contains(keyword.ToLower())).OrderBy(t => t.OrderNumber).ThenByDescending(t => t.PublishedDate);
                //foreach (var post in category.Posts)
                //{
                //    bool exist = posts.Any(t => t.Id == post.Id);
                //    if (!exist)
                //    {
                //        posts.Add(post);
                //    }
                //}
                var _posts = category.PostCategories.Select(p => p.Post).ToList();
                posts.AddRange(_posts);
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
            var searchPost = new List<Post>();
            foreach (var item in posts)
            {
                if (item.Title.ToLower().Contains(keyword.ToLower()))
                {
                    searchPost.Add(item);
                }
            }
            var orderPost = searchPost.OrderBy(t => t.OrderNumber).ThenByDescending(t => t.PublishedDate).Distinct().ToList();
            return View(orderPost);
        }
    }
}