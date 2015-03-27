using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormulaWebApplication.Startup))]
namespace FormulaWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
