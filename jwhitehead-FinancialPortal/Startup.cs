using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jwhitehead_FinancialPortal.Startup))]
namespace jwhitehead_FinancialPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
