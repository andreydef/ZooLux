using System.Web.Mvc;
using System.Web.Routing;

namespace ZooLux
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "",
                new { controller = "Pages", action = "Index" },
                new[] { "ZooLux.Controllers" });

            routes.MapRoute("Pages", "{page}", 
                new { controller = "Pages", action = "Index" },
                new[] { "ZooLux.Controllers" });

            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                new[] { "ZooLux.Controllers" });
        }
    }
}