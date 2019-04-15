using IEE.Infrastructure;
using IEE.Infrastructure.DbContext;
using IEE.Web.Areas.ttn_content.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace IEE.Web.Models
{
    public class Utils
    {
        #region Singleton Implementation
        // The variable is declared to be volatile to ensure that assignment to the instance variable completes before the instance variable can be accessed
        private static volatile Utils _instance;

        // Uses a syncRoot instance to lock on, rather than locking on the type itself, to avoid deadlocks.
        private static object syncRoot = new Object();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Utils();
                    }
                }
                return _instance;
            }
        }
        #endregion

        //private Infrastructure.DbContext.SATEntities db = new Infrastructure.DbContext.SATEntities();
        public List<IEE.Infrastructure.DbContext.Category> GetListMenu()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var categories = unitOfWork.DataContext.Categories.Include("ChildCategory").Where(t => t.ParentId == null && (t.IsDeleted == null || t.IsDeleted == false) && (t.IsMenu == true)).OrderBy(t => t.OrderNumber).ToList();
                return categories;
            }
        }
        public List<string> GetListPosition()
        {
            return new List<string>() { "Home", "Header", "Footer", "Right" };
        }
        public List<string> GetListDisplayType()
        {
            return new List<string>() { "List", "Blog", "Grid", "Content" };
        }

        public List<Infrastructure.DbContext.Banner> GetListBanner()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var banners = unitOfWork.DataContext.Banners.Where(b => b.IsDeleted == false).OrderBy(b => b.BannerIndex).ToList();

                return banners;
            }
        }
        public List<IEE.Infrastructure.DbContext.Category> GetListRightMenu()
        {
            using (var dbContext = new UnitOfWork())
            {


                var _categoryRepo = dbContext.GetRepository<Infrastructure.DbContext.Category>();
                var ieltCat = _categoryRepo.GetMany(c => c.ParentId == 5).Where(c => c.Name == "IELTS Foundation" || c.Name == "IELTS 5.5" || c.Name == "IELTS 6.5" || c.Name == "IELTS >7.5").OrderBy(c => c.OrderNumber).ToList();
                var satCat = _categoryRepo.GetMany(c => c.ParentId == 11).Where(c => c.Name == "SAT Beginner" || c.Name == "SAT Intermediate" || c.Name == "SAT Advanced" || c.Name == "SAT Super Advanced").OrderBy(c => c.OrderNumber).ToList();
                ieltCat.AddRange(satCat);
                return ieltCat;
            }
        }

        public string GetSectionName(int section)
        {
            switch (section)
            {
                case (int)CommonConstants.EnumSection.Reading:
                    return CommonConstants.EnumSection.Reading.ToString();
                case (int)CommonConstants.EnumSection.Writing:
                    return CommonConstants.EnumSection.Writing.ToString();
                case (int)CommonConstants.EnumSection.Math_NoCalc:
                    return "Math";
                case (int)CommonConstants.EnumSection.Math_Calc:
                    return "Math with calculator";
                default:
                    return string.Empty;
            }
        }

        public int GetSectionDuration(int section)
        {
            switch (section)
            {
                case (int)CommonConstants.EnumSection.Reading:
                    return (int)CommonConstants.EnumSectionDuration.Reading;
                case (int)CommonConstants.EnumSection.Writing:
                    return (int)CommonConstants.EnumSectionDuration.Writing;
                case (int)CommonConstants.EnumSection.Math_NoCalc:
                    return (int)CommonConstants.EnumSectionDuration.Math_NoCalc;
                case (int)CommonConstants.EnumSection.Math_Calc:
                    return (int)CommonConstants.EnumSectionDuration.Math_Calc;
                default:
                    return 0;
            }

        }
        public int GetSectionScore(int sectionId)
        {
            using (var db = new SATEntities())
            {
                var section = db.SATTestSections.FirstOrDefault(s => s.ID == sectionId);
                if (section != null)
                {
                    return db.SATScores.FirstOrDefault(sc => sc.ID == section.ScoreID).ScoreValue;
                }
                return 0;
            }
        }

        public string RenderViewToString(string viewPath, object model)
        {
            ControllerContext context = CreateController<GenericController>().ControllerContext;

            // first find the ViewEngine for this view, note assumes only partials
            ViewEngineResult viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            //string result = null;

            //using (var sw = new StringWriter())
            //{
            //    var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);

            //    view.Render(ctx, sw);
            //    result = sw.ToString();
            //}

            ViewEngineResult partViewResult = ViewEngines.Engines.FindPartialView(context, viewPath);

            if (partViewResult.View != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(context, partViewResult.View, context.Controller.ViewData, context.Controller.TempData, output);
                        partViewResult.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }
            return String.Empty;
            // return result;
        }


        public T CreateController<T>(RouteData routeData = null)
            where T : Controller, new()
        {
            // create a disconnected controller instance
            T controller = new T();

            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
            {
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            }
            else
            {
                throw new InvalidOperationException("Can't create Controller Context if no active HttpContext instance is available.");
            }

            if (routeData == null)
            {
                routeData = new RouteData();
            }

            // add the controller routing if not existing
            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
            {
                routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", string.Empty));
            }

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);

            return controller;
        }

        public string GetVNName(string name)
        {
            switch (name)
            {
                case "Home":
                    return "Trang chủ";
                case "Header":
                    return "Đầu trang";
                case "Footer":
                    return "Cuối trang";
                case "Right":
                    return "Bên phải";
                case "Blog":
                    return "Blog";
                case "Grid":
                    return "Lưới";
                case "List":
                    return "Danh sách";
                case "Content":
                    return "Nội dung";
                default:
                    return string.Empty;
            }
        }

        public string ReturnPartialContent(PartialViewModel model)
        {
            switch (model.Cate.DisplayType)
            {
                case "Blog":
                    return RenderViewToString("~/Views/Shared/_BlogType.cshtml", model);
                case "List":
                    return RenderViewToString("~/Views/Shared/_ListType.cshtml", model);
                case "Grid":
                    return RenderViewToString("~/Views/Shared/_GridType.cshtml", model);
                case "Content":
                    return RenderViewToString("~/Views/Shared/_ContentType.cshtml", model);
                default:
                    return RenderViewToString("~/Views/Shared/_ListType.cshtml", model);
            }
        }

        public string CreateFolderIfNeeded(string filePath)
        {
            var di = new DirectoryInfo(filePath);
            if (!di.Exists)
            {
                di.Create();

                return filePath;
            }
            return filePath;

        }

        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    public class GenericController : Controller
    {

    }
    public class BaseModel
    {
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
    public static class CustomMapper
    {
        public static T Map<T>(this object entity) where T : new()
        {
            if (entity != null)
            {
                var convert = new T();
                var sourceProps = entity.GetType().GetProperties().Where(x => x.CanRead).ToList();
                var destProps = typeof(T).GetProperties()
                        .Where(x => x.CanWrite)
                        .ToList();
                foreach (var sourceProp in sourceProps)
                {
                    if (destProps.Any(x => x.Name == sourceProp.Name))
                    {
                        var p = destProps.First(x => x.Name == sourceProp.Name);
                        if (p.CanWrite)
                        {
                            p.SetValue(convert, sourceProp.GetValue(entity, null), null);
                        }
                    }
                }
                return convert;
            }
            else
            {
                return default(T);
            }
        }
        public static TEntity MapUpdateWithStatus<TEntity>(this object model, int userId, TEntity entity) where TEntity : new()
        {
            var baseModel = new BaseModel() { ModifiedDate = DateTime.Now, ModifiedBy = userId, IsDeleted = false };

            if (model != null)
            {
                var sourceProps = model.GetType().GetProperties().Where(x => x.CanRead).ToList();
                var destProps = typeof(TEntity).GetProperties()
                        .Where(x => x.CanWrite)
                        .ToList();
                var _base = baseModel.GetType().GetProperties().Where(x => x.CanRead).ToList();

                foreach (var sourceProp in sourceProps)
                {
                    if (sourceProp.Name == "ModifiedDate" || sourceProp.Name == "ModifiedBy" || sourceProp.Name == "IsDeleted")
                    {
                        if (destProps.Any(x => x.Name == sourceProp.Name))
                        {
                            var p = destProps.First(x => x.Name == sourceProp.Name);
                            var val = _base.First(x => x.Name == sourceProp.Name).GetValue(baseModel);

                            if (p.CanWrite)
                            {
                                p.SetValue(entity, val, null);
                            }
                        }
                    }
                    else
                    {
                        if (destProps.Any(x => x.Name == sourceProp.Name))
                        {
                            var p = destProps.First(x => x.Name == sourceProp.Name);
                            if (p.CanWrite)
                            {
                                p.SetValue(entity, sourceProp.GetValue(model, null), null);
                            }
                        }

                    }

                }

                return entity;

            }
            else
            {
                return default(TEntity);
            }

        }
    }

    public static class StringHelper
    {
        public static int ToInt(this string str)
        {
            return Convert.ToInt32(str);
        }
    }

    public class DisabledSection
    {
        public List<int> Section { get; set; }
        public int DisabledInExam { get; set; }
    }
}