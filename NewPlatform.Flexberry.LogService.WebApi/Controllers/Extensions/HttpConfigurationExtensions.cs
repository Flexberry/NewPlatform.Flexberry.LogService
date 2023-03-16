#if NETFRAMEWORK
namespace ICSSoft.STORMNET.Controllers.Extensions
{
    using System;
    using System.Web.Http;

    /// <summary>
    /// Класс, содержащий расширения для сервиса логирования.
    /// </summary>
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Maps the specified Log Service route.
        /// </summary>
        /// <param name="config">The current HTTP configuration.</param>
        /// <param name="routeName">The name of the route.</param>
        /// <param name="routeTemplate">The template of the route to map.</param>
        public static void MapLogControllerRoute(
            this HttpConfiguration config,
            string routeName = "logs",
            string routeTemplate = "api/logs")
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Parameter requirement not met: config != null");
            }

            if (routeName == null)
            {
                throw new ArgumentNullException(nameof(routeName), "Parameter requirement not met: routeName != null");
            }

            if (routeName == string.Empty)
            {
                throw new ArgumentException("Parameter requirement not met: routeName != string.Empty", nameof(routeName));
            }

            if (routeTemplate == null)
            {
                throw new ArgumentNullException(nameof(routeTemplate), "Parameter requirement not met: routeTemplate != null");
            }

            if (routeTemplate == string.Empty)
            {
                throw new ArgumentException("Parameter requirement not met: routeTemplate != string.Empty", nameof(routeTemplate));
            }

            config.Routes.MapHttpRoute(routeName, routeTemplate, defaults: new { controller = "Logs", action = "PostLog" });
        }
    }
}
#endif