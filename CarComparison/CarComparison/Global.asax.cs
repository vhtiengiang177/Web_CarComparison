using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CarComparison
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;

        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");           //Remove Server Header  
            Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
        }

        void Session_Start(Object sender, EventArgs e)
        {
            Response.Cookies["ASP.NET_SessionId"].SameSite = SameSiteMode.Lax;
            //while we're at it lets also make it secure
            if (Request.IsSecureConnection)
                Response.Cookies["ASP.NET_SessionId"].Secure = true;
        }


    }
}
