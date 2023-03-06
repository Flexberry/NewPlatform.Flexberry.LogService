namespace ICSSoft.STORMNET.Controllers
{
    using ICSSoft.STORMNET.RequestsObjects;

#if NETSTANDARD
    using Microsoft.AspNetCore.Mvc;
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
        public StatusCodeResult PostLog(LogRequest request)
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
}
