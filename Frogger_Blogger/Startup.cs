using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Frogger_Blogger.Startup))]
namespace Frogger_Blogger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
