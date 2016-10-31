using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FeedbackSystem.Startup))]
namespace FeedbackSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
