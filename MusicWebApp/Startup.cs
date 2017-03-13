using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MusicWebApp.Startup))]
namespace MusicWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
