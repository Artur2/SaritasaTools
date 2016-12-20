using Microsoft.Owin;
using Owin;
using Saritasa.Tools.Configuration;

[assembly: OwinStartup(typeof(ZergRushCo.Todosya.Web.Startup))]
namespace ZergRushCo.Todosya.Web
{
    /// <summary>
    /// Owin startup class.
    /// </summary>
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigSystem.Install("ZergRushCo.Todosya.Web.dll.config");
            ConfigureAuth(app);
        }
    }
}
