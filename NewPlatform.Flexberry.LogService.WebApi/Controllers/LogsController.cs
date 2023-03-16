namespace ICSSoft.STORMNET.Controllers
{
    using ICSSoft.STORMNET.RequestsObjects;
    using Newtonsoft.Json;

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
    public class LogsController : ControllerBase
#endif
#if NETFRAMEWORK
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
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string msg = JsonConvert.SerializeObject(request, settings);

            try
            {
                switch (request.Category.ToLower())
                {
                    case "info":
                        LogService.LogInfo(msg);
                        break;
                    case "debug":
                        LogService.LogDebug(msg);
                        break;
                    case "warn":
                    case "warning":
                        LogService.LogWarn(msg);
                        break;
                    case "error":
                        LogService.LogError(msg);
                        break;
                    default:
                        LogService.LogInfo(msg);
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