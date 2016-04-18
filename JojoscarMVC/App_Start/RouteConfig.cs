
using System.Web.Mvc;
using System.Web.Routing;

namespace JojoscarMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Guest", action = "Index", year = "2016" }
            );

            routes.MapRoute(
                name: "action",
                url: "{controller}/{action}/{year}/{id}",
                defaults: new { year = "", id = UrlParameter.Optional }
            );
        }
    }
}
