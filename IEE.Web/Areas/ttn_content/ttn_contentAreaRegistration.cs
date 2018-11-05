using System.Web.Mvc;

namespace IEE.Web.Areas.ttn_content
{
    public class ttn_contentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ttn_content";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ttn_content_default",
                "ttn_content/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}