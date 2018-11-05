using System;
using System.Collections.Generic;
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
using IEE.Web.Business;
using IEE.Infrastructure;
using System.Threading.Tasks;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    //[CustomAuthorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]
    public class PostController : BaseController
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Category> _categoryRepository;
        private UnitOfWork _unitOfWork;
        public PostController()
        {
            _unitOfWork = new UnitOfWork();
            _categoryRepository = _unitOfWork.GetRepository<Category>();
            _postRepository = _unitOfWork.GetRepository<Post>();
        }
        // GET: ttn_content/Post
        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _postRepository.GetMany(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Title.ToLower().Contains(keyword.ToLower()));
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
            var categories = _categoryRepository.GetMany(t => t.IsDeleted == false && t.ParentId.HasValue == false && t.IsMenu == true);
            ViewBag.Categories = categories;

            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public async Task<ActionResult> Create(PostViewModel model, string[] selectedCategories, string submit, FormCollection collection)
        {
            if (submit.Equals("Cancel"))
            {
                return RedirectToAction("index");
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    var categories = _categoryRepository.GetMany(t => t.IsDeleted == false && t.ParentId.HasValue == false && t.IsMenu == true);
                    ViewBag.Categories = categories;

                    return View(model);
                }

                var allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
                //process upload an image
                string file_name = string.Empty, file_src = string.Empty;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        file_name = Guid.NewGuid() + Path.GetExtension(fileName);
                        file_src = Path.Combine(Server.MapPath("~/photo/post/"), file_name);
                        file.SaveAs(file_src);
                        break;
                    }
                }
                var post = Mapper.Map<PostViewModel, Post>(model);
                post.Photo = file_name;
                var maxId = await Task.Run(()=> _postRepository.GetAll().Max(p => p.Id) + 1);
                post.Id = maxId;
                post.CreatedBy = post.ModifiedBy = User.UserId;
                post.CreatedDate = post.ModifiedDate = DateTime.Now;
                post.ModifiedDate = DateTime.Now;
                post.IsDeleted = false;

                var attachFiles = collection.Get("AttachFile");
                post.AttachFiles = attachFiles;

                switch (submit)
                {
                    case "Save":
                        post.Status = false;
                        _postRepository.InsertAndSubmit(post);
                        var newPost = _postRepository.GetById(post.Id);
                        AddCatPost(selectedCategories, newPost);
                        return RedirectToAction("index");
                    case "Preview":
                        post.Status = false;
                        _postRepository.InsertAndSubmit(post);
                        var new_Post = _postRepository.GetById(post.Id);
                        AddCatPost(selectedCategories, new_Post);
                        return RedirectToAction("preview", "news", new { area = "", id = post.Id });
                    case "Publish":
                        post.PublishedDate = DateTime.Now;
                        post.Status = true;
                        _postRepository.InsertAndSubmit(post);
                        var _newPost = _postRepository.GetById(post.Id);
                        AddCatPost(selectedCategories, _newPost);
                        return RedirectToAction("index");
                }
            }
            return RedirectToAction("index");
        }

        private void AddCatPost(string[] selectedCategories, Post post)
        {
            //process category
            if (selectedCategories != null && selectedCategories.Count() > 0)
            {
                int[] uniInts = selectedCategories.Select(int.Parse).ToArray();
                var categories = _categoryRepository.GetMany(t => uniInts.Contains(t.Id));
                using (var db = new UnitOfWork())
                {
                    var catPostRepo = db.GetRepository<PostCategory>();


                    foreach (var u in categories)
                    {
                        var _catPost = new PostCategory { CategoryID = u.Id, PostID = post.Id };
                        catPostRepo.InsertAndSubmit(_catPost);
                    }
                }
            }
        }

        public ActionResult Edit(int id)
        {
            var post = _postRepository.Get(t => t.Id == id);
            var model = Mapper.Map<Post, PostViewModel>(post);

            var categories = _categoryRepository.GetMany(t => t.IsDeleted == false && t.ParentId.HasValue == false && t.IsMenu == true);
            ViewBag.Categories = categories;

            ViewBag.SelectedCategories = post.PostCategories.Select(c => c.Category).ToList();

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(PostViewModel model, string[] selectedCategories, string submit, FormCollection collection)
        {
            if (submit.Equals("Cancel"))
            {
                return RedirectToAction("index");
            }
            else
            {
                if (!ModelState.IsValid || selectedCategories == null || selectedCategories.Count() == 0)
                {
                    //ViewBag.SelectedCategories = _postRepository.Get(t => t.Id == model.Id).PostCategories.Select(c => c.Category);
                    ViewBag.SelectedCategories = new List<Category>();
                    ModelState.AddModelError("", "Hãy chọn danh mục cho bài viết");
                    var categories = _categoryRepository.GetMany(t => t.IsDeleted == false && t.ParentId.HasValue == false && t.IsMenu == true);
                    ViewBag.Categories = categories;

                    ModelState.AddModelError("", "Bạn hãy kiểm tra lại các thông tin vừa nhập");
                    return View(model);
                }
                Post post = _postRepository.Get(t => t.Id == model.Id);

                post.Title = model.Title;
                post.Summarize = model.Summarize;
                post.Detail = model.Detail;
                post.Status = model.Status;

                var allowedExtensions = new string[] { ".jpeg", ".jpg", ".png" };
                string file_name = string.Empty, file_src = string.Empty;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        file_name = Guid.NewGuid() + Path.GetExtension(fileName);
                        file_src = Path.Combine(Server.MapPath("~/photo/post/"), file_name);
                        file.SaveAs(file_src);

                        break;
                    }
                }

                if (!string.IsNullOrEmpty(file_name))
                    post.Photo = file_name;
                post.MetaKey = model.MetaKey;
                post.MetaDescription = model.MetaDescription;
                post.ModifiedBy = User.UserId;
                post.ModifiedDate = DateTime.Now;
                post.PublishedDate = post.PublishedDate;
                post.IsDeleted = false;
                UpdatePostCategories(_unitOfWork,selectedCategories, post);

                post.AttachFiles = "";
                post.AttachFiles = collection.Get("AttachFile");
                switch (submit)
                {
                    case "Save":
                        _postRepository.UpdateAndSubmit(post);
                        _unitOfWork.Commit();
                        return RedirectToAction("index");
                    case "Preview":
                        _postRepository.UpdateAndSubmit(post);
                        _unitOfWork.Commit();
                        return RedirectToAction("preview", "news", new { area = "", id = post.Id });
                    case "Publish":
                        post.PublishedDate = DateTime.Now;
                        post.Status = true;
                        _postRepository.UpdateAndSubmit(post);
                        _unitOfWork.Commit();
                        return RedirectToAction("index");
                }

            }


            return RedirectToAction("index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var post = _postRepository.Get(t => t.Id == id);
                var catPost = post.PostCategories.ToList();
                using (var unitOfWork = new UnitOfWork())
                {
                    var catPostRepo = unitOfWork.GetRepository<PostCategory>();

                    foreach (var item in catPost)
                    {
                        catPostRepo.DeleteAndSubmit(item);
                    }

                }
                _postRepository.DeleteAndSubmit(post);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }

        public ActionResult DeleteAll()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var post = _postRepository.GetAll();
                    var catPostRepo = unitOfWork.GetRepository<PostCategory>();

                    foreach (var item in post)
                    {
                        var listPostCat = catPostRepo.GetMany(c => c.PostID == item.Id).ToList();
                        foreach (var catPostitem in listPostCat)
                        {
                            catPostRepo.DeleteAndSubmit(catPostitem);
                        }
                        _postRepository.Delete(item.Id);
                    }

                    _postRepository.SaveChanges();
                }

                return RedirectToAction("index");
            }
            catch (Exception ex)
            {

                return RedirectToAction("index");
            }
        }

        public FileResult Download(string fileName)
        {
            var filepath = Path.Combine(Server.MapPath("/Photo/Post/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

        private void ProcessContent(PostViewModel model, string[] selectedCategories)
        {

        }
        private void UpdatePostCategories(UnitOfWork unitOfWork,string[] selectedCategories, Post postToUpdate)
        {
            try
            {
                if (selectedCategories.Count() == 0)
                {
                    var listCate = new List<Category>();
                    return;
                }

                
                    var catPostRepo = unitOfWork.GetRepository<PostCategory>();
                    var listToDelete = catPostRepo.GetMany(cp => cp.PostID == postToUpdate.Id).ToList();

                    foreach (var deleteItem in listToDelete)
                    {
                        catPostRepo.Delete(deleteItem);
                    }
                    foreach (var item in selectedCategories)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var model = new PostCategory { CategoryID = int.Parse(item), PostID = postToUpdate.Id };
                            catPostRepo.Insert(model);
                        }
                    }

                
            }
            catch (Exception ex)
            {

                
            }
        }
    }
}