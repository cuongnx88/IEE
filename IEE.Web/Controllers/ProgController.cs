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

namespace IEE.Web.Controllers
{
    public class ProgController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;
        public ProgController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _postRepo = unitOfWork.GetRepository<Post>();
        }
        // GET: Program
        public ActionResult Index(string keyword)
        {
            var category = _categoryRepo.Get(t => t.keyword.Equals(keyword));
            var post = category.PostCategories.Select(p => p.Post).Where(t => t.Status != null && t.Status.Value).OrderByDescending(t => t.Id).FirstOrDefault();
            ViewBag.Category = category;
            return View(post);
        }

        public ActionResult Group(string program)
        {
            var listCat = _categoryRepo.GetAll();
            var catModel = new Category();
            foreach (var item in listCat)
            {
                if (item.Name.ToSeoUrl() == program.ToLower() && item.IsDeleted == false)
                {
                    catModel = item;
                    break;
                }
            }
            //var category = _categoryRepo.Get(t => t.Id == id);
            //catModel.Posts = _postRepo.GetMany(t => t.Status == true && t.Title.Contains("SAT")).ToList();
            //var _Post = _postRepo.GetMany(t => t.Status == true && t.Title.Contains("SAT")).OrderByDescending(o => o.Id).FirstOrDefault();
            if (catModel != null && catModel.PostCategories.Select(p => p.Post) != null)
            {
                var postCat = catModel.PostCategories.Select(p => p.Post).ToList();
                ViewBag.Category = catModel;
                var post = postCat.Where(t => t != null && t.Status == true).OrderByDescending(o => o.Id).FirstOrDefault();
                return View(post);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }



        }
    }
}