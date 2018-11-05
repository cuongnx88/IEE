using Autofac;
using Autofac.Integration.Mvc;

using IEE.Infrastructure;
using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using IEE.Service;
using System.Web.Mvc;
using IEE.Web.Mappings;
using IEE.WebApp.Mappings;
using AutoMapper;

namespace IEE.Web
{
    public class Bootstrapper
    {
        public static void Run()
        {
            // SetAutofacContainer();
            //Configure AutoMapper
            DomainToViewModelMappingProfile.Instance.Configure();
            ViewModelToDomainMappingProfile.Instance.Configure();
           
        }

        //private static void SetAutofacContainer()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.RegisterControllers(Assembly.GetExecutingAssembly());
        //    // Register modules
        //    //builder.RegisterModule(new MvcSiteMapProviderModule()); // Required
        //    //builder.RegisterModule(new MvcModule()); // Required by MVC. Typically already part of your setup (double check the contents of the module).


        //    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        //    builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(BannerRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(CategoryRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(PostRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
            
        //    builder.RegisterAssemblyTypes(typeof(RoleRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(SettingRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
            
        //    builder.RegisterAssemblyTypes(typeof(ConsultantRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
            
        //    builder.RegisterAssemblyTypes(typeof(TestimonialRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
        //    builder.RegisterAssemblyTypes(typeof(ConsultantRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
        //    builder.RegisterAssemblyTypes(typeof(CouncilRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(UniversityHighlightRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(StudentHighlightRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(TeacherHighlightRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
        //    builder.RegisterAssemblyTypes(typeof(CalendarRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(MediaRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();
        //    builder.RegisterAssemblyTypes(typeof(ProgramRepository).Assembly)
        //        .Where(t => t.Name.EndsWith("Repository"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(MembershipService).Assembly)
        //        .Where(t => t.Name.EndsWith("Service"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    builder.RegisterAssemblyTypes(typeof(EncryptionService).Assembly)
        //        .Where(t => t.Name.EndsWith("Service"))
        //            .AsImplementedInterfaces().InstancePerLifetimeScope();

        //    //builder.RegisterAssemblyTypes(typeof(DefaultFormsAuthentication).Assembly)
        //    //.Where(t => t.Name.EndsWith("Authentication"))
        //    //.AsImplementedInterfaces().InstancePerHttpRequest();

        //    ////builder.Register(c => new UserManager<User>(new UserStore<User>(new ERPChanLapEntities())))
        //    ////    .As<UserManager<User>>().InstancePerHttpRequest();

        //    builder.RegisterFilterProvider();
        //    IContainer container = builder.Build();
        //    //// Setup global sitemap loader (required)
        //    //MvcSiteMapProvider.SiteMaps.Loader = container.Resolve<ISiteMapLoader>();

        //    //// Check all configured .sitemap files to ensure they follow the XSD for MvcSiteMapProvider (optional)
        //    //var validator = container.Resolve<ISiteMapXmlValidator>();
        //    //validator.ValidateXml(HostingEnvironment.MapPath("~/Mvc.sitemap"));

        //    //// Register the Sitemaps routes for search engines (optional)
        //    //XmlSiteMapController.RegisterRoutes(RouteTable.Routes);

        //    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        //}
    }
}