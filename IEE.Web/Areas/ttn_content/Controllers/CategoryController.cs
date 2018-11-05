using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Service.Abstract;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;
using IEE.Model;
using AutoMapper;
using System.Configuration;
using PagedList;
using IEE.Infrastructure;
using System.Data.Entity;
using IEE.Web.Models;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly UnitOfWork _unitOfWork;
        public CategoryController()
        {
            _unitOfWork = new UnitOfWork();
            _categoryRepo = _unitOfWork.GetRepository<Category>();

        }
        // GET: ttn_content/Category
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var parentModel = _categoryRepo.GetMany(c => (c.IsDeleted == null || c.IsDeleted == false) && c.Name != "Bài viết" && c.ParentId == null).OrderByDescending(c=>c.IsSystem).ThenBy(c=>c.OrderNumber).ToList();

           
            var returnModel= new List<Category>();
   
            foreach (var item in parentModel)
            {
                var childCate = item.ChildCategory.Where(c=>c.IsDeleted==null||c.IsDeleted==false).OrderBy(c => c.OrderNumber).ToList();
                returnModel.Add(item);
                returnModel.AddRange(childCate);
            }

            //var model = _categoryRepo.GetMany(c => c.IsDeleted == null || c.IsDeleted == false && c.Name != "Bài viết").OrderByDescending(ord => ord.IsSystem).ThenByDescending(c=>c.ParentId).ThenBy(c=>c.OrderNumber).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                returnModel = returnModel.Where(k => keyword.ToLower().Equals(k.Name.ToLower())).ToList();
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
            return View(returnModel.ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));
        }

        public ActionResult Create()
        {
            List<Category> categories = GetListMenu();
            ViewBag.Categories = categories;
            return View();
        }

        private List<Category> GetListMenu()
        {
            var parents = _categoryRepo.GetMany(t => t.ParentId == null && (t.IsDeleted == null || t.IsDeleted == false) && !t.Name.Contains("Bài viết")).ToList();

            ViewBag.Parents = parents;

            List<Category> categories = new List<Category>();
            foreach (var item in parents)
            {
                item.Name = item.Name + "-------------";
                categories.Add(item);
                foreach (var child in _categoryRepo.GetMany(c => c.ParentId == item.Id).ToList())
                {
                    categories.Add(child);
                }
            }

            return categories;
        }

        [HttpPost]
        public ActionResult Create(CategoryViewModel model, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                //var parents = _categoryRepo.GetMany(t => t.ParentId == null);
                //ViewBag.Parents = parents;

                List<Category> categories = GetListMenu();
                
                ViewBag.Categories = categories;

                return View(model);
            }
            var isParent = collection.Get("isParent");
            if (isParent != null && isParent == "true")
            {
                model.ParentId = null;
            }
            model.Position = collection.Get("CatPosition");
            model.DisplayType = collection.Get("DisplayType");
            var category = Mapper.Map<CategoryViewModel, Category>(model);
            if (category.ParentId != null)
            {
                category.Controller = _categoryRepo.Get(t => t.Id == category.ParentId).Controller;
            }
            else
            {
                category.Controller = null;
            }

            category.CreatedBy = User.UserId;
            category.ModifiedBy = User.UserId;
            category.CreatedDate = category.ModifiedDate = DateTime.Now;
            category.IsDeleted = false;
            category.SEOTitle = category.Name.ToSeoUrl();
            _categoryRepo.InsertAndSubmit(category);

            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            using (var db = new SATEntities())
            {
                //var parents = db.Categories.Where(t => t.ParentId == null).ToList().Select(c => c.Map<CategoryModel>()).ToList();
                //ViewBag.Parents = parents;
                Category category = db.Categories.Find(id);
                List<Category> categories = GetListMenu();
               
                ViewBag.Categories = categories;


                if (category.ParentId == null)
                {
                    ViewBag.isParent = true;
                }
                else
                {
                    ViewBag.isParent = false;
                }
                //var unProxyCate = DynamicProxyHelpers.UnProxy<Category>(_unitOfWork.DataContext, category);
                //var model = Mapper.Map<Category, CategoryViewModel>(unProxyCate);
                var model = category.Map<CategoryViewModel>();
                return View(model);
            }

        }
        [HttpPost]
        public ActionResult Edit(CategoryViewModel model, int id, FormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                //var parents = _categoryRepo.GetMany(t => t.ParentId == null);
                //ViewBag.Parents = parents;

                List<Category> categories = GetListMenu();
                
                ViewBag.Categories = categories;

                return View(model);
            }
            var catEntity = _categoryRepo.GetById(id);
            var category = Mapper.Map<CategoryViewModel, Category>(model);
            if (catEntity.ParentId != null)
            {
                var parentCat = _categoryRepo.Get(t => t.Id == category.ParentId);
                category.Controller = parentCat == null ? null : parentCat.Controller;

            }
            category.Position = collection.Get("CatPosition");
            category.ModifiedDate = DateTime.Now;
            category.ModifiedBy = User.UserId;
            category.IsDeleted = false;
            category.SEOTitle = category.Name.ToSeoUrl();
            if (collection.Get("isParent") != null && collection.Get("isParent") == "true")
            {
                category.ParentId = null;
            }
            if (collection.Get("DisplayType") != null)
            {
                category.DisplayType = collection.Get("DisplayType");
            }
            // _categoryRepo.UpdateAndSubmit(category);

            using (var db = new SATEntities())
            {
                var attachCat = db.Categories.Attach(category);
                db.Entry(attachCat).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("index");

        }
        public ActionResult Delete(int id)
        {
            var category = _categoryRepo.Get(t => t.Id == id && t.IsSystem == false);
            if (category != null)
            {
                category.ModifiedDate = DateTime.Now;
                category.ModifiedBy = User.UserId;
                category.IsDeleted = true;
            }

            _categoryRepo.UpdateAndSubmit(category);
            return RedirectToAction("index");
        }
    }
}