using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TipTournament.Startup))]
namespace TipTournament
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
