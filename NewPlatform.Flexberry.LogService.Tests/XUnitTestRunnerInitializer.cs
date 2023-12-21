[assembly: Xunit.TestFramework("NewPlatform.Flexberry.LogService.Tests.XUnitTestRunnerInitializer", "NewPlatform.Flexberry.LogService.Tests")]

namespace NewPlatform.Flexberry.LogService.Tests
{
#if NETCOREAPP
    using log4net;
    using log4net.Config;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
#endif
    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Инициализация тестового запуска.
    /// </summary>
    public class XUnitTestRunnerInitializer : XunitTestFramework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XUnitTestRunnerInitializer" /> class.
        /// </summary>
        /// <param name="messageSink">The message sink used to send diagnostic messages.</param>
        public XUnitTestRunnerInitializer(IMessageSink messageSink)
            : base(messageSink)
        {
#if NETCOREAPP
            string configFile = $"{Assembly.GetExecutingAssembly().Location}.config";
            string outputConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            File.Copy(configFile, outputConfigFile, true);
            
            // Init logging using correct configuration file:
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(outputConfigFile));
#endif
        }
    }
}
