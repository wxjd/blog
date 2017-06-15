using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NiitBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
               name: "User",
               url: "{username}",
               defaults: new { controller = "User", action = "Index" },
               constraints: new { controller = "User", action = "Index" }
           );

            routes.MapRoute(
                name: "Manage",
                url: "{username}/{action}",
                defaults: new { controller = "User", action = "Manage" },
                constraints: new { controller = "User", action = "Manage" }
            );

            routes.MapRoute(
              name: "Album",
              url: "{username}/Album",
              defaults: new { controller = "Album", action = "Index" },
              constraints: new { controller = "Album", action = "Index" }
            );

            routes.MapRoute(
              name: "Photo",
              url: "{username}/Album/{albumid}",
              defaults: new { controller = "Album", action = "Photo" },
              constraints: new { controller = "Album", action = "Photo", albumid = @"\d+" }
            );

            routes.MapRoute(
                name: "Article",
                url: "{username}/{controller}/{action}/{id}",
                defaults: new { controller = "Article", id = UrlParameter.Optional },
                constraints: new { action = @"Detail|Post|Postedit" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { action = @"^(?!Detail$).+$" }
            );
        }
    }
}
