using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmokingLog.Startup))]
namespace SmokingLog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
