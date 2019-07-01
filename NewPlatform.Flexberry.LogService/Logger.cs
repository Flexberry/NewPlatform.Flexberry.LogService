namespace ICSSoft.STORMNET
{
    using System;

    using log4net;

    /// <summary>
    /// Общий сервис для ведения логов (смотри документацию по log4net: http://logging.apache.org/log4net/).
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// Собственно логгер (log4net).
        /// </summary>
        public static ILog Log = LogManager.GetLogger("Flexberry", "Flexberry");

        #region Обёртка для Log.Error

        /// <summary>
        /// Обёртка для Log.Error, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        public static void LogError(object message)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(message);
            }
        }

        /// <summary>
        /// Обёртка для Log.Error, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        /// <param name="exception">
        /// Исключение, которое будет передано в логгер.
        /// </param>
        public static void LogError(object message, Exception exception)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error(message, exception);
            }
        }

        /// <summary>
        /// Обёртка для Log.ErrorFormat, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="format">
        /// Формат записи.
        /// </param>
        /// <param name="args">
        /// Аргументы, которые будут переданы в логгер.
        /// </param>
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (Log.IsErrorEnabled)
            {
                Log.ErrorFormat(format, args);
            }
        }

        #endregion Обёртка для Log.Error

        #region Обёртка для Log.Info

        /// <summary>
        /// Обёртка для Log.Info, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        public static void LogInfo(object message)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(message);
            }
        }

        /// <summary>
        /// Обёртка для Log.Info, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        /// <param name="exception">
        /// Исключение, которое будет передано в логгер.
        /// </param>
        public static void LogInfo(object message, Exception exception)
        {
            if (Log.IsInfoEnabled)
            {
                Log.Info(message, exception);
            }
        }

        /// <summary>
        /// Обёртка для Log.InfoFormat, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="format">
        /// Формат записи.
        /// </param>
        /// <param name="args">
        /// Аргументы, которые будут переданы в логгер.
        /// </param>
        public static void LogInfoFormat(string format, params object[] args)
        {
            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat(format, args);
            }
        }

        #endregion Обёртка для Log.Info

        #region Обёртка для Log.Warn

        /// <summary>
        /// Обёртка для Log.Warn, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        public static void LogWarn(object message)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(message);
            }
        }

        /// <summary>
        /// Обёртка для Log.Warn, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        /// <param name="exception">
        /// Исключение, которое будет передано в логгер.
        /// </param>
        public static void LogWarn(object message, Exception exception)
        {
            if (Log.IsWarnEnabled)
            {
                Log.Warn(message, exception);
            }
        }

        /// <summary>
        /// Обёртка для Log.InfoFormat, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="format">
        /// Формат записи.
        /// </param>
        /// <param name="args">
        /// Аргументы, которые будут переданы в логгер.
        /// </param>
        public static void LogWarnFormat(string format, params object[] args)
        {
            if (Log.IsWarnEnabled)
            {
                Log.WarnFormat(format, args);
            }
        }

        #endregion Обёртка для Log.Warn

        #region Обёртка для Log.Debug

        /// <summary>
        /// Обёртка для Log.Warn, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        public static void LogDebug(object message)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(message);
            }
        }

        /// <summary>
        /// Обёртка для Log.Warn, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="message">
        /// Сообщение, которое будет передано в логгер.
        /// </param>
        /// <param name="exception">
        /// Исключение, которое будет передано в логгер.
        /// </param>
        public static void LogDebug(object message, Exception exception)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug(message, exception);
            }
        }

        /// <summary>
        /// Обёртка для Log.InfoFormat, включающая соответствующую проверку, что логирование включено.
        /// </summary>
        /// <param name="format">
        /// Формат записи.
        /// </param>
        /// <param name="args">
        /// Аргументы, которые будут переданы в логгер.
        /// </param>
        public static void LogDebugFormat(string format, params object[] args)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(format, args);
            }
        }

        #endregion Обёртка для Log.Debug
    }
}
