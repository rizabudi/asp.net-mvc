using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Helpers
{
    public static class NavigationIndicatorHelper
    {
        public static string MakeActiveClass(this IUrlHelper urlHelper, string controller, string action)
        {
            try
            {
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("controller"))
                {
                    return null;
                }
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("action"))
                {
                    return null;
                }

                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                {
                    if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string MakeActiveClass(this IUrlHelper urlHelper, List<string> controllers, string action)
        {
            try
            {
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("controller"))
                {
                    return null;
                }
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("action"))
                {
                    return null;
                }

                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                foreach(string controller in controllers)
                {
                    if (controller.Equals(controllerName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                        {
                            return result;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string MakeActiveClass(this IUrlHelper urlHelper, List<string> controllers, List<string> actions)
        {
            try
            {
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("controller"))
                {
                    return null;
                }
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("action"))
                {
                    return null;
                }

                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                foreach (string controller in controllers)
                {
                    if (controller.Equals(controllerName, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (string action in actions)
                        {
                            if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                            {
                                return result;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string MakeOpenClass(this IUrlHelper urlHelper, List<string> controllers, string action)
        {
            try
            {
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("controller"))
                {
                    return null;
                }
                if (!urlHelper.ActionContext.RouteData.Values.ContainsKey("action"))
                {
                    return null;
                }

                string result = "open menu-open";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                foreach (string controller in controllers)
                {
                    if (controller.Equals(controllerName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                        {
                            return result;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string MakeVisibleClass(this IUrlHelper urlHelper, bool status)
        {
            return status ? "" : "none";
        }
    }
}
