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
    public class AbroadController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;
        public AbroadController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>();
            _postRepo = unitOfWork.GetRepository<Post>();
        }
        // GET: Program
        public ActionResult Index()
        {
            var category = _categoryRepo.Get(t => t.Id == 4);
            var post = category.PostCategories.Select(p=>p.Post).OrderByDescending(t => t.Id).FirstOrDefault();
            return View(post);
        }

        public ActionResult Group(int id)
        {
            var category = _categoryRepo.Get(t => t.Id == id);
            var post = category.PostCategories.Select(p => p.Post).OrderByDescending(t => t.Id).FirstOrDefault();
            return View(post);
        }
    }
}