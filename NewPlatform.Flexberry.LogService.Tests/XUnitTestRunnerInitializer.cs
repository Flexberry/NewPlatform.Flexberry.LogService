[assembly: Xunit.TestFramework("NewPlatform.Flexberry.LogService.Tests.XUnitTestRunnerInitializer", "NewPlatform.Flexberry.LogService.Tests")]

namespace NewPlatform.Flexberry.LogService.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
#if NETCOREAPP
    using System.Configuration;
    using log4net;
    using log4net.Config;
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
            // Copy App.config into testhost.dll.config:
            string configFile = $"{Assembly.GetExecutingAssembly().Location}.config";
            string outputConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            File.Copy(configFile, outputConfigFile, true);

            // Refresh log4net configuration:
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(outputConfigFile));
#endif

            // Copy other test configurations from Log4net.Configs folder:
            string entryAssemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            string exeAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Utils.CopyFilesRecursively(Path.Combine(entryAssemblyPath, "Log4net.Configs"), exeAssemblyPath);
        }
    }
}
