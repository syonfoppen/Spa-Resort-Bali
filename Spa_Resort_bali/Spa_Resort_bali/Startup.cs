using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Spa_Resort_bali.Startup))]

namespace Spa_Resort_bali
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}