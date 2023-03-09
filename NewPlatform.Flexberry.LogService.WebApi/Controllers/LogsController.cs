namespace ICSSoft.STORMNET.Controllers
{
    using ICSSoft.STORMNET.RequestsObjects;

#if NETSTANDARD
    using Microsoft.AspNetCore.Mvc;
#endif
#if NETFRAMEWORK
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
#endif

    /// <summary>
    /// WebAPI controller for Flexberry Log Service.
    /// </summary>
#if NETSTANDARD
    [Produces("application/json")]
    public class LogsController : ControllerBase
    {
        /// <summary>
        /// Send log message for LogService.
        /// </summary>
        /// <param name="request">Log object.</param>
        /// <returns>Returns <c>true</c> if LogService write log<c>false</c> if error.</returns>
        [HttpPost]
        [ActionName("PostLog")]
        public StatusCodeResult PostLog([FromBody] LogRequest request)
        {
            try
            {
                LogService.LogDebug(request.LogMessage);
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
#endif
#if NETFRAMEWORK
    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        /// <summary>
        /// Send log message for LogService.
        /// </summary>
        /// <param name="request">Log object.</param>
        [HttpPost]
        [ActionName("PostLog")]
        public void PostLog([FromBody] LogRequest request)
        {
            try
            {
                LogService.LogDebug(request.LogMessage);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
#endif
}
