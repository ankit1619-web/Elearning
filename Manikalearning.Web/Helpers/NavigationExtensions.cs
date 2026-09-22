using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manikalearning.Web.Helpers
{
    public static class NavigationExtensions
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string? action = null, string? area = "Admin")
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();
            var currentArea = routeData.Values["area"]?.ToString();

            bool areaMatch = string.IsNullOrEmpty(area) || string.Equals(currentArea, area, StringComparison.OrdinalIgnoreCase);
            bool controllerMatch = string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase);
            bool actionMatch = string.IsNullOrEmpty(action) || string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);

            return (areaMatch && controllerMatch && actionMatch) ? "active" : "";
        }

        public static string IsMenuShow(this IHtmlHelper htmlHelper, params string[] controllers)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return controllers.Any(c => string.Equals(currentController, c, StringComparison.OrdinalIgnoreCase)) ? "show" : "";
        }

        public static string IsMenuCollapsed(this IHtmlHelper htmlHelper, params string[] controllers)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return controllers.Any(c => string.Equals(currentController, c, StringComparison.OrdinalIgnoreCase)) ? "" : "collapsed";
        }
    }
}
