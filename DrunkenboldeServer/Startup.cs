using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DrunkenboldeServer.Startup))]

namespace DrunkenboldeServer
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
