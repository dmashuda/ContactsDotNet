using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Contacts.Startup))]
namespace Contacts
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
