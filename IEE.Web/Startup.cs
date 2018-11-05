using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEE.Web.Startup))]
namespace IEE.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
