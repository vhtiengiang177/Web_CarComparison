﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CarComparison
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "trangchu",
                url: "Home",
                defaults: new { controller = "Client", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "sosanh",
                url: "Comparison",
                defaults: new { controller = "Client", action = "Comparing", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Client", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
