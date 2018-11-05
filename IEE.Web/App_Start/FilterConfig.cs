using System.Web;
using System.Web.Mvc;

namespace IEE.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new IEE.Web.Filter.PostViewCountAttribute());
        }
    }
}
