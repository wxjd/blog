using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NiitBlog.Startup))]
namespace NiitBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
