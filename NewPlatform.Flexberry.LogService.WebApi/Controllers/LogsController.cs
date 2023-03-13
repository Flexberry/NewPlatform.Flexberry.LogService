namespace ICSSoft.STORMNET.Controllers
{
    using ICSSoft.STORMNET.RequestsObjects;

#if NETSTANDARD
    using Microsoft.AspNetCore.Mvc;
#endif
#if NETFRAMEWORK
    using System.Net;
    using System.Web.Http;
#endif

    /// <summary>
    /// WebAPI controller for Flexberry Log Service.
    /// </summary>
#if NETSTANDARD
    [Produces("application/json")]
    public class LogsController : ControllerBase
#endif
#if NETFRAMEWORK
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
#endif
    {
#if NETSTANDARD
        /// <summary>
        /// Send log message for LogService.
        /// </summary>
        /// <param name="request">Log object.</param>
        /// <returns>Returns <c>true</c> if LogService write log<c>false</c> if error.</returns>
        [HttpPost]
        [ActionName("PostLog")]
        public StatusCodeResult PostLog([FromBody] LogRequest request)
#endif
#if NETFRAMEWORK
        /// <summary>
        /// Send log message for LogService.
        /// </summary>
        /// <param name="request">Log object.</param>
        [HttpPost]
        [ActionName("PostLog")]
        public void PostLog([FromBody] LogRequest request)
#endif
        {
            try
            {
                switch (request.Category.ToLower())
                {
                    case "info":
                        LogService.LogInfo(request.Message);
                        break;
                    case "debug":
                        LogService.LogDebug(request.Message);
                        break;
                    case "warn":
                    case "warning":
                        LogService.LogWarn(request.Message);
                        break;
                    case "error":
                        LogService.LogDebug(request.Message);
                        break;
                    default:
                        LogService.LogInfo(request.Message);
                        break;
                }
            }
            catch
            {
#if NETSTANDARD
                return BadRequest();
#endif
#if NETFRAMEWORK
                throw new HttpResponseException(HttpStatusCode.BadRequest);
#endif
            }

#if NETSTANDARD
            return Ok();
#endif
        }
    }
}