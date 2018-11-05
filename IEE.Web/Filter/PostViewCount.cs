using AutoMapper;
using IEE.Infrastructure;
using IEE.Infrastructure.DbContext;
using IEE.Model;
using IEE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace IEE.Web.Filter
{
    public class PostViewCountAttribute : ActionFilterAttribute
    {
        private static readonly TimeSpan pageViewDumpToDatabaseTimeSpan = new TimeSpan(0, 0, 30);
        public PostViewCountAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            try
            {

                //danh cho count page view 
                var calledAction = string.Format("{0}/{1}/{2}",
                                                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                                                filterContext.ActionDescriptor.ActionName, filterContext.RouteData.Values["id"] == null ? null : filterContext.RouteData.Values["id"].ToString());

                var cacheActionKey = string.Format("PV-{0}", calledAction);

                var cachedActionResult = HttpRuntime.Cache[cacheActionKey];

                if (cachedActionResult == null)
                {
                    var currentValue = new PageViewValue();
                    int artID;
                    var controller = filterContext.RouteData.Values["controller"];
                    var action = filterContext.RouteData.Values["action"];
                    var id = filterContext.RouteData.Values["id"];
                    if (controller.ToString() == "News" && action.ToString() == "Content" && id != null)
                    {
                        artID = int.Parse(id.ToString());

                        var sessID = filterContext.HttpContext.Session;
                        currentValue = new PageViewValue(artID);
                        currentValue.Value++;
                        if (filterContext.HttpContext.Session["viewSessionID"]!=null)
                        {
                            currentValue.SessionId = filterContext.HttpContext.Session["viewSessionID"].ToString();
                        }
                        else
                        {
                            sessID.Add("viewSessionID", sessID.SessionID);
                            currentValue.SessionId = filterContext.HttpContext.Session["viewSessionID"].ToString();
                        }
                       
                       
                        HttpRuntime.Cache.Insert(cacheActionKey, currentValue, null, DateTime.Now.Add(pageViewDumpToDatabaseTimeSpan), Cache.NoSlidingExpiration, CacheItemPriority.Default,
                                              onRemove);
                    }
                    

                    //currentValue.Value++;
                    //HttpRuntime.Cache.Insert(cacheActionKey, currentValue, null, DateTime.Now.Add(pageViewDumpToDatabaseTimeSpan), Cache.NoSlidingExpiration, CacheItemPriority.Default,
                    //                      onRemove);
                }
                else
                {
                    var currentValue = (PageViewValue)cachedActionResult;

                    currentValue.Value++;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            base.OnActionExecuting(filterContext);

        }
        private void onRemove(string key, object value, CacheItemRemovedReason reason)
        {


            if (key.StartsWith("PV-News/Content"))
            {

                // view count cua bai viet

                var id = 0;
                string myK = key.Split('/')[2];

                try
                {
                    id = Convert.ToInt32(myK);
                }
                catch
                {
                    id = 0;

                }

                // write out the value to the database


                if (id != 0)
                {
                    var dbfactor = new DatabaseFactory();


                    using (UnitOfWork unitOfWork = new UnitOfWork())
                    {
                      
                        var postRepo = unitOfWork.GetRepository<Post>();
                        var viewSessRepo = unitOfWork.GetRepository<ViewSession>();
                        var postModel = postRepo.GetById(id);
                        var cVar = (PageViewValue)value;
                        var viewSession = unitOfWork.DataContext.ViewSessions.Where(v => v.PostId == id && v.SessionId == cVar.SessionId).ToList();
                       // var viewSession = viewSessRepo.GetMany(v => v.PostId == id && v.SessionId == cVar.SessionId);
                        if (viewSession.Count()>0)
                        {
                            return;
                        }
                        else
                        {
                            var viewSessModel = new ViewSession();
                            viewSessModel.SessionId = cVar.SessionId;
                            viewSessModel.PostId = id;
                            viewSessRepo.InsertAndSubmit(viewSessModel);

                            postModel.ViewCount = cVar.Value;
                            postRepo.UpdateAndSubmit(postModel);
                        }
                      
                        return;
                    }
                }


            }

        }
    }
    // Used to get around weird cache behavior with value types
    public class PageViewValue
    {
        public long? Value { get; set; }
        public string SessionId { get; set; }
        public PageViewValue()
        {
            Value = 0;
        }
        public PageViewValue(int id)
        {
            IDatabaseFactory factory = new DatabaseFactory();
            UnitOfWork unitOfWork = new UnitOfWork(factory);
            var post = unitOfWork.DataContext.Posts.Find(id);

            Value = post.ViewCount.HasValue ? post.ViewCount.Value : 0;


        }

    }
}