#if NETSTANDARD
namespace ICSSoft.STORMNET.Controllers.Extensions
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides extension methods for <see cref="IRouteBuilder"/> to add Log Service Web API route.
    /// </summary>
    public static class MapRouteRouteBuilderExtensions
    {
        /// <summary>
        /// Maps the specified Log Service Web API route.
        /// </summary>
        /// <param name="builder">The <see cref="IRouteBuilder"/> to add the route to.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="routeTemplate">The template of the route to map.</param>
        /// <returns>Request result as <see cref="IRouteBuilder"/>.</returns>
        public static IRouteBuilder MapLogsRoute(
            this IRouteBuilder builder,
            string routeName = "logs",
            string routeTemplate = "api/logs")
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder), "Parameter requirement not met: builder != null");
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

            var applicationPartManager = builder.ServiceProvider.GetRequiredService<ApplicationPartManager>();
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(typeof(LogsController).Assembly));

            return builder.MapRoute(routeName, routeTemplate, defaults: new { controller = "Logs", action = "PostLog" });
        }
    }
}
#endif
