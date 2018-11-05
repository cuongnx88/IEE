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
using System.Data.Entity;
using IEE.Web.Models;

namespace IEE.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Post> _postRepo;
        private readonly IRepository<Council> _councilRepo;
        private readonly IRepository<StudentHighlight> _studentRepo;
        private readonly IRepository<TeacherHighlight> _teacherRepo;
        private readonly IRepository<UniversityHighlight> _universityRepo;
        public AboutController()
        {
            var unitOfWork = new UnitOfWork();
            _categoryRepo = unitOfWork.GetRepository<Category>(); 
            _postRepo = unitOfWork.GetRepository<Post>();
            _councilRepo = unitOfWork.GetRepository<Council>();
            _studentRepo = unitOfWork.GetRepository<StudentHighlight>();
            _teacherRepo = unitOfWork.GetRepository<TeacherHighlight>();
            _universityRepo= unitOfWork.GetRepository<UniversityHighlight>();
        }
        // GET: About
        public ActionResult Index()
        {
            var category = _categoryRepo.Get(t => t.Controller.Equals("about") && t.IsDeleted == false && t.ParentId == 1);
            var post = category.PostCategories.Select(p => p.Post).OrderByDescending(t => t.Id).FirstOrDefault();
            return View(post);
        }

        public ActionResult Group(int id)
        {
            var category = _categoryRepo.Get(t => t.Id==id && t.IsDeleted == false);
            var post = category.PostCategories.Select(p => p.Post).OrderByDescending(t => t.Id).FirstOrDefault();
            return View(post);
        }

        public ActionResult Council()
        {
            var councils = _councilRepo.GetMany(t => t.IsDeleted == false);   
            return View(councils);
        }

        public ActionResult Students()
        {
            var controller = Request.Path;
            
                
            var students = _studentRepo.GetMany(t => t.IsDeleted == false);
            switch (controller)
            {
                case "/top-sat":
                    students = students.Where(sat=>sat.SAT!=null).OrderByDescending(s => s.SAT).ToList();
                    ViewBag.TopCat = "Top Sat";
                    return View(students);
                case "/top-ielt":
                    students = students.Where(ielt=>ielt.IELTS!=null).OrderByDescending(s => s.IELTS).ToList();
                    ViewBag.TopCat = "Top IELT";
                    return View(students);
                case "/top-toefl":
                    var orderStd= students.Where(toefl=>toefl.TOEFL!=null).Select(s=>new { s,ord=s.TOEFL.ToInt() }).OrderByDescending(s=>s.ord).ToList();
                    ViewBag.TopCat = "Top TOEFL";
                    return View(orderStd.Select(s=>s.s).ToList());
                default:
                    return View(students); ;
            }
           
           
        }
        public ActionResult Teachers()
        {
            var teachers = _teacherRepo.GetMany(t => t.IsDeleted == false);
            return View(teachers);
        }
        public ActionResult Schools()
        {
            var schools = _universityRepo.GetMany(t => t.IsDeleted == false).OrderBy(s=>s.Ranking).ToList();
            return View(schools);
        }

        public ActionResult Thanks()
        {
            return View();
        }
    }
}