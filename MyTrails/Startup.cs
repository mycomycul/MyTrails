using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyTrails.Startup))]
namespace MyTrails
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }

    }
}
