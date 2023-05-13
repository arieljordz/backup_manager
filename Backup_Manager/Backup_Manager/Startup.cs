using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Backup_Manager.Startup))]
namespace Backup_Manager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}