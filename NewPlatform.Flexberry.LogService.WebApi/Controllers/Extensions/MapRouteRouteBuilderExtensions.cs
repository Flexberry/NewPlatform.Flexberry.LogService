#if NETSTANDARD
namespace ICSSoft.STORMNET.Controllers.Extensions
{
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
        /// <returns>Request result as <see cref="IRouteBuilder"/>.</returns>
        public static IRouteBuilder MapLogsRoute(
            this IRouteBuilder builder,
            string routeName = "logs")
        {
            var applicationPartManager = builder.ServiceProvider.GetRequiredService<ApplicationPartManager>();
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(typeof(LogsController).Assembly));

            return builder.MapRoute(routeName, "api/logs", defaults: new { controller = "Logs", action = "PostLog" });
        }
    }
}
#endif
