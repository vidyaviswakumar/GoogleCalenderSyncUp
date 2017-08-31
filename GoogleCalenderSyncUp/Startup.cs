using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoogleCalenderSyncUp.Startup))]
namespace GoogleCalenderSyncUp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
