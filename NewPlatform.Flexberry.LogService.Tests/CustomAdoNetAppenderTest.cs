namespace NewPlatform.Flexberry.LogService.Tests
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using ICSSoft.STORMNET;

    using log4net;
    using log4net.Config;

    using Xunit;

    /// <summary>
    /// Класс, содержащий модульные тесты для Log4net аппендера <see cref="CustomAdoNetAppender"/>,
    /// поддерживающего получение строк соединения из конфигурационного файла приложения (в т.ч. зашифрованных строк соединения).
    /// </summary>
    public class CustomAdoNetAppenderTest
    {
        /// <summary>
        /// Строка соединения.
        /// </summary>
        private const string ApplicationConnectionString = "SERVER=MyServer; TRUSTED_CONNECTION=no; DATABASE=MyDataBase; USER ID = MyUser; PASSWORD=MyPassword";

        /// <summary>
        /// Проверка логики шифрования и дешифрования.
        /// </summary>
        [Fact]
        public void EncryptDecryptConnectionStringInAppender()
        {
            // Arrange & Act.
            string encriptedConnectionString = CustomAdoNetAppender.EncryptString(ApplicationConnectionString, true);
            string actual = CustomAdoNetAppender.DecryptString(encriptedConnectionString, true);

            // Assert.
            Assert.Equal(ApplicationConnectionString, actual);
        }

        /// <summary>
        /// Test for reading connection string from appsettings.json and App.config.
        /// </summary>
        [Fact]
        public void ReadConnectionStringTest()
        {
            // Prepare
            PrepareLog4Net("App.config");

            // Act
            var adoNetAppender = LogService.Log.Logger.Repository.GetAppenders().First(x => x is CustomAdoNetAppender) as CustomAdoNetAppender;
            string connStr = adoNetAppender.GetConnectionStringFromConfiguration();

            // Assert
            Assert.NotNull(connStr);
#if NETCOREAPP
            Assert.DoesNotContain("SERVER=app.config.string", connStr);
#else
            Assert.Contains("SERVER=app.config.string", connStr);
#endif
        }

        /// <summary>
        /// Test for utilizing DefaultConnectionStringName parameter in App.config.
        /// </summary>
        [Fact]
        public void ReadDefaultConnectionStringTest()
        {
            // Prepare
            PrepareLog4Net("App.defconnstr.config");

            // Act
            var adoNetAppender = LogService.Log.Logger.Repository.GetAppenders().First(x => x is CustomAdoNetAppender) as CustomAdoNetAppender;
            string connStr = adoNetAppender.GetConnectionStringFromConfiguration();

            // Assert
            Assert.NotNull(connStr);
            Assert.Contains("SERVER=app.config.defconnstr", connStr);
        }

        /// <summary>
        /// Test for utilizing CustomizationStrings parameter when no connection string is specified in App.config.
        /// </summary>
        [Fact]
        public void ReadCustomizationStringsTest()
        {
            // Prepare
            PrepareLog4Net("App.customizationstrings.config");

            // Act
            var adoNetAppender = LogService.Log.Logger.Repository.GetAppenders().First(x => x is CustomAdoNetAppender) as CustomAdoNetAppender;
            string connStr = adoNetAppender.GetConnectionStringFromConfiguration();

            // Assert
            Assert.NotNull(connStr);
            Assert.Contains("SERVER=app.config.customizationstrings", connStr);
        }

        /// <summary>
        /// Initialize log4net with specified configuration file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        private void PrepareLog4Net(string fileName)
        {
            // Replace testhost.dll.config with specified configuration file:
            var exeAssembly = Assembly.GetExecutingAssembly();
            string configFilePath = Path.Combine(Path.GetDirectoryName(exeAssembly.Location), fileName);
            string outputConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            File.Copy(configFilePath, outputConfigFile, true);

            // Refresh sections for log4net initialization:
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");

            var logRepository = LogManager.GetRepository(exeAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo(configFilePath));
        }
    }
}
