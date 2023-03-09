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
        public static void MapLogControllerRoute(
            this HttpConfiguration config,
            string routeName = "logs")
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "Contract assertion not met: config != null");
            }

            if (routeName == null)
            {
                throw new ArgumentNullException(nameof(routeName), "Contract assertion not met: routeName != null");
            }

            if (routeName == string.Empty)
            {
                throw new ArgumentException("Contract assertion not met: routeName != string.Empty", nameof(routeName));
            }

            config.Routes.MapHttpRoute(routeName, "api/logs", defaults: new { controller = "Logs", action = "PostLog" });
        }
    }
}
#endif