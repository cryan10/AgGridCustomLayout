using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LoanAppPOC.Startup))]
namespace LoanAppPOC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
