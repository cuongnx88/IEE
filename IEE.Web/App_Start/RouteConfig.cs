using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LowercaseDashedRouting;

namespace IEE.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "About",
                "gioi-thieu",
                new { controller = "About", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               "Contact",
               "lien-he",
               new { controller = "Contact", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                "GroupAbout",
                "gioi-thieu/{id}/{alias}",
                new { controller = "About", action = "Group", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "Council",
                "hoi-dong-thanh-vien",
                new { controller = "About", action = "Council", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    "GroupAbroad",
            //    "du-hoc-my/{id}/{alias}",
            //    new { controller = "Abroad", action = "Group", id = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    "Abroad",
            //    "du-hoc-my/{Category}",
            //    new { controller = "Abroad", action = "Index", Category = UrlParameter.Optional }
            //);

            routes.MapRoute(
                "Consult",
                "dang-ky-tu-van",
                new { controller = "Consult", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               "SATRegister",
               "SATOnline/dang-ky",
               new { controller = "SATAccount", action = "Register" , id = UrlParameter.Optional },
                new[] { "IEE.Web.Areas.ttn_content.Controllers" }
           );
            routes.MapRoute(
              "SATLogin",
              "SATOnline/Login",
              new { controller = "SATAccount", action = "Login", id = UrlParameter.Optional },
               new[] { "IEE.Web.Areas.ttn_content.Controllers" }
          );
            routes.MapRoute(
             "SATTestIndex",
             "SATOnline",
             new { controller = "SATTest", action = "Index", id = UrlParameter.Optional }
            
         );
            routes.MapRoute(
           "SATTest",
           "SATOnline/Test",
           new { controller = "SATTest", action = "Test", id = UrlParameter.Optional }

       );
            routes.MapRoute(
                "Calendars",
                "lich-hoc",
                new { controller = "Calendars", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "GroupProgram",
                "chuong-trinh/{program}",
                new { controller = "Prog", action = "Group", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Event",
                "su-kien",
                new { controller = "Event", action = "Index", id = UrlParameter.Optional }
            );
            

            routes.MapRoute(
                "News",
                "tin-tuc",
                new { controller = "News", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Testimonial",
              url: "Cam-nhan-hoc-vien",
              defaults: new
              {
                  controller = "Testimonial",
                  action = "List",
                  id = UrlParameter.Optional
              }
          );

            routes.MapRoute(
               name: "TestimonialDetails",
               url: "Cam-nhan-hoc-vien/{student}-{id}",
               defaults: new
               {
                   controller = "Testimonial",
                   action = "Details",
                   id = UrlParameter.Optional,
                   student = UrlParameter.Optional
               }
           );
            routes.MapRoute(
              "AllTeacher",
              "doi-ngu-giao-vien",
              new { controller = "About", action = "Teachers" }
          );
            routes.MapRoute(
               "TopSchool",
               "top-schools",
               new { controller = "About", action = "Schools" }
           );
            routes.MapRoute(
              "TopSat",
              "top-sat",
              new { controller = "About", action = "Students" }
          );
            routes.MapRoute(
             "TopIelt",
             "top-ielt",
             new { controller = "About", action = "Students" }
         );
            routes.MapRoute(
            "TopToefl",
            "top-toefl",
            new { controller = "About", action = "Students" }
        );

            routes.MapRoute(
                name: "Homepage",
                url: "",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                }
            );
            
            routes.MapRoute(
                "CatSEORoute",
                "{Category}",
                 new { controller = "News", action = "Category",Category= UrlParameter.Optional }
                );
            routes.MapRoute(
                "NewsSEORoute",
                "{category}/{title}-{id}",
                 new { controller = "News", action = "Content", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                "DetailNews",
                "tin-tuc/{id}/{alias}",
                new { controller = "News", action = "Content", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "GroupNews",
                "tin-tuc/{id}/{pageNum}/{alias}",
                new { controller = "News", action = "Category", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               "AllVideo",
               "video/all",
               new { controller = "TutorialVideo", action = "AllVideo" }
           );

            routes.MapRoute(
                "ListVideo",
                "video/{catName}",
                new { controller = "TutorialVideo", action = "ListVideo", catName = UrlParameter.Optional }
            );
          
            routes.MapRoute(
                "TutorVideo",
                "{catName}/{videoName}-{id}",
                new { controller = "TutorialVideo", action = "ShowContent", id = UrlParameter.Optional}
            );
           

            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{title}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional,
                    title = UrlParameter.Optional }
            );
          
            routes.LowercaseUrls = true;
        }
    }
}
