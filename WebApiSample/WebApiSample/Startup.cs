using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApiSample;

[assembly: OwinStartup(typeof(Startup))]
namespace WebApiSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            // Get your HttpConfiguration. In OWIN, you'll create one rather than using GlobalConfiguration.
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
        }
    }
}