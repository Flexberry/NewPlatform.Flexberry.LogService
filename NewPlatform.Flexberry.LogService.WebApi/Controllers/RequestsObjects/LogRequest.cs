namespace ICSSoft.STORMNET.RequestsObjects
{
    using System;

    /// <summary>
    /// Log request class.
    /// </summary>
    public class LogRequest
    {
        /// <summary>
        /// The category of log record.
        /// Expected values: "info", "debug", "warn", "warning", "error".
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The event ID.
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// The priority.
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// The severity.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// The title of log record.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The timestamp of log record.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// The machine name.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The application damain name.
        /// </summary>
        public string AppDomainName { get; set; }

        /// <summary>
        /// The process ID.
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// The process name.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// The thread name.
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// The win32 thread ID.
        /// </summary>
        public string Win32ThreadId { get; set; }

        /// <summary>
        /// The message of log record.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The formatted message of log record.
        /// </summary>
        public string FormattedMessage { get; set; }
    }
}
