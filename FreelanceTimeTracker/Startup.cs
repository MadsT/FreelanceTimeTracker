using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FreelanceTimeTracker.Startup))]
namespace FreelanceTimeTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
