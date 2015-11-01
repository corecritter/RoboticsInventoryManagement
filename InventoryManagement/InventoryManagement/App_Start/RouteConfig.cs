using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InventoryManagement
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                 //defaults: new { controller = "ItemTypes", action = "Index", id = UrlParameter.Optional }
                 //defaults: new { controller = "InventoryLocations", action = "Index", id = UrlParameter.Optional }
                 defaults: new { controller = "Labels", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
