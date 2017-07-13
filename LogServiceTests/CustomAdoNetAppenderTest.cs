namespace LogService.Tests
{
    using ICSSoft.STORMNET;

    using System.Collections.Specialized;
    using System.Configuration;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.QualityTools.Testing.Fakes;

    /// <summary>
    /// Класс, содержащий модульные тесты для Log4net аппендера <see cref="CustomAdoNetAppender"/>,
    /// поддерживающего получение строк соединения из конфигурационного файла приложения (в т.ч. зашифрованных строк соединения).
    /// </summary>
    [TestClass]
    public class CustomAdoNetAppenderTest
    {
        /// <summary>
        /// Имя строки соединения в конфигурационной секции connectionStrings.
        /// </summary>
        private const string ApplicationConnectionStringName = "DefConnStr";

        /// <summary>
        /// Строка соединения.
        /// </summary>
        private const string ApplicationConnectionString = "SERVER=MyServer; TRUSTED_CONNECTION=no; DATABASE=MyDataBase; USER ID = MyUser; PASSWORD=MyPassword";

        /// <summary>
        /// Имя дополнительной строки соединения в конфигурационной секции connectionStrings.
        /// </summary>
        private const string AdditionalConnectionStringName = "AddConnStr";

        /// <summary>
        /// Дополнительная строка соединения.
        /// </summary>
        private const string AdditionalConnectionString = "SERVER=MyAddServer; TRUSTED_CONNECTION=no; DATABASE=MyAddDataBase; USER ID = MyAddUser; PASSWORD=MyAddPassword";

        /// <summary>
        /// Имя строки соединения несуществующей в конфигурационной секции connectionStrings.
        /// </summary>
        private const string NonExistentConnectionStringName = "NonExistentConnectionStringName";

        /// <summary>
        /// Осуществляет создание тестовой оболочки для конфигурации приложения.
        /// </summary>
        /// <param name="appSettingConnectionStringName">
        /// Значение настройки <see cref="CustomAdoNetAppender.AppSettingConnectionStringName"/> в секции appSettings.
        /// </param>
        /// <param name="appSettingConnectionString">
        /// Значение настройки <see cref="CustomAdoNetAppender.AppSettingConnectionString"/> в секции appSettings.
        /// </param>
        /// <param name="appSettingConnectionStringIsEncrypted">
        /// Значение настройки <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> в секции appSettings.
        /// </param>
        /// <param name="encryptConnectionString">
        /// Флаг: нужно ли зашифровать строки соединения.
        /// </param>
        private void CreateApplicationConfigurationShims(
            string appSettingConnectionStringName = null, 
            string appSettingConnectionString = null, 
            string appSettingConnectionStringIsEncrypted = null,
            bool encryptConnectionString = false)
        {
            var appSettings = new NameValueCollection();
            if (!string.IsNullOrEmpty(appSettingConnectionStringName))
            {
                appSettings.Add(CustomAdoNetAppender.AppSettingConnectionStringName, appSettingConnectionStringName);
            }

            if (!string.IsNullOrEmpty(appSettingConnectionString))
            {
                appSettingConnectionString = encryptConnectionString
                        ? CustomAdoNetAppender.EncryptString(appSettingConnectionString, true)
                        : appSettingConnectionString;
                appSettings.Add(CustomAdoNetAppender.AppSettingConnectionString, appSettingConnectionString);
            }

            if (!string.IsNullOrEmpty(appSettingConnectionStringIsEncrypted))
            {
                appSettings.Add(CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted, appSettingConnectionStringIsEncrypted);
            }

            var appConnectionStrings = new ConnectionStringSettingsCollection();
            string applicationConnectionString = encryptConnectionString
                        ? CustomAdoNetAppender.EncryptString(ApplicationConnectionString, true)
                        : ApplicationConnectionString;
            string additionalConnectionString = encryptConnectionString
                ? CustomAdoNetAppender.EncryptString(AdditionalConnectionString, true)
                : AdditionalConnectionString;
            appConnectionStrings.Add(new ConnectionStringSettings(ApplicationConnectionStringName, applicationConnectionString));
            appConnectionStrings.Add(new ConnectionStringSettings(AdditionalConnectionStringName, additionalConnectionString));

            System.Configuration.Fakes.ShimConfigurationManager.AppSettingsGet = () => { return appSettings; };
            System.Configuration.Fakes.ShimConfigurationManager.ConnectionStringsGet = () => { return appConnectionStrings; };
        }

        /// <summary>
        /// Осуществляет имитацию того, что у приложения есть <see cref="System.Web.HttpContext"/>.
        /// Т.е. имитирует работу из web-окружения. 
        /// </summary>
        private void EmulateWebEnvironment()
        {
            // Имитируем работу из web-приложения.
            var request = new System.Web.Hosting.SimpleWorkerRequest(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                new System.IO.StringWriter());
            System.Web.Fakes.ShimHttpContext.CurrentGet = () => { return new System.Web.HttpContext(request); };
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по переданному ему имени.
        /// </summary>
        public void GetConnectionStringByNameInAppender()
        {           
            // Задаем строки соединения в секции connectionStrings.
            CreateApplicationConfigurationShims();

            var appender1 = new CustomAdoNetAppender { ConnectionStringName = ApplicationConnectionStringName };
            Assert.IsTrue(appender1.ConnectionString == ApplicationConnectionString);

            var appender2 = new CustomAdoNetAppender { ConnectionStringName = AdditionalConnectionStringName };
            Assert.IsTrue(appender2.ConnectionString == AdditionalConnectionString);
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по переданному ему имени.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        public void WinGetConnectionStringByNameInAppender()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringByNameInAppender();
            }
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по переданному ему имени.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        public void WebGetConnectionStringByNameInAppender()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringByNameInAppender();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если ему было передано имя несуществующей строки соединения.
        /// </summary>
        public void GetConnectionStringByNameInAppenderFail()
        {
            // Задаем строки соединения в секции connectionStrings.
            CreateApplicationConfigurationShims();

            // Задаем аппендеру имя несуществующей строки соединения.
            var appender = new CustomAdoNetAppender { ConnectionStringName = NonExistentConnectionStringName };
            string connectionString = appender.ConnectionString;
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если ему было передано имя несуществующей строки соединения.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WinGetConnectionStringByNameInAppenderFail()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringByNameInAppenderFail();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если ему было передано имя несуществующей строки соединения.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WebGetConnectionStringByNameInAppenderFail()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringByNameInAppenderFail();
            }
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по имени, которое было указано в конфигурационном файле приложения, 
        /// если имя не было передано аппендеру напрямую.
        /// </summary>
        public void GetConnectionStringByNameInAppSettings()
        {
            // Задаем строки соединения в секции connectionStrings, и имя основной строки в секции appSettings.
            CreateApplicationConfigurationShims(ApplicationConnectionStringName);

            // Не задаем аппендеру имя строки соединения.
            // Он должен взять его из конфигурационного файла.
            var appender = new CustomAdoNetAppender();
            Assert.IsTrue(appender.ConnectionString == ApplicationConnectionString);
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по имени, которое было указано в конфигурационном файле приложения, 
        /// если имя не было передано аппендеру напрямую.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        public void WinGetConnectionStringByNameInAppSettings()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringByNameInAppSettings();
            }
        }

        /// <summary>
        /// Аппендер должен получать строку соединения по имени, которое было указано в конфигурационном файле приложения, 
        /// если имя не было передано аппендеру напрямую.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        public void WebGetConnectionStringByNameInAppSettings()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringByNameInAppSettings();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в конфигурационном файле приложения, указано имя несуществующей строки соединения,
        /// и корректное имя не было передано аппендеру напрямую.
        /// </summary>
        public void GetConnectionStringByNameInAppSettingsFail()
        {
            // Задаем строки соединения в секции connectionStrings, и имя несуществующей строки в секции appSettings.
            CreateApplicationConfigurationShims(NonExistentConnectionStringName);

            // Не задаем аппендеру имя строки соединения.
            // Он должен взять его из конфигурационного файла.
            var appender = new CustomAdoNetAppender();
            string connectionString = appender.ConnectionString;
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в конфигурационном файле приложения, указано имя несуществующей строки соединения,
        /// и корректное имя не было передано аппендеру напрямую.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WinGetConnectionStringByNameInAppSettingsFail()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringByNameInAppSettingsFail();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в конфигурационном файле приложения, указано имя несуществующей строки соединения,
        /// и корректное имя не было передано аппендеру напрямую.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WebGetConnectionStringByNameInAppSettingsFail()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringByNameInAppSettingsFail();
            }
        }

        /// <summary>
        /// Аппендер должен получать строку соединения прямо из конфигурационном файла приложения, 
        /// если имя строки нигде не указано, но строка задана напрямую в секции appSettings.
        /// </summary>
        public void GetConnectionStringInAppSettings()
        {
            // Не задаем никакого имени строки соединения, но задаем строку соединения прямо в секции appSettings.
            CreateApplicationConfigurationShims(null, ApplicationConnectionString);

            // Не задаем аппендеру имя строки соединения.
            // Он должен взять строку прямо из секции appSettings конфигурационного файла.
            var appender = new CustomAdoNetAppender();
            Assert.IsTrue(appender.ConnectionString == ApplicationConnectionString);
        }

        /// <summary>
        /// Аппендер должен получать строку соединения прямо из конфигурационном файла приложения, 
        /// если имя строки нигде не указано, но строка задана напрямую в секции appSettings.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        public void WinGetConnectionStringInAppSettings()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringInAppSettings();
            }
        }

        /// <summary>
        /// Аппендер должен получать строку соединения прямо из конфигурационном файла приложения, 
        /// если имя строки нигде не указано, но строка задана напрямую в секции appSettings.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        public void WebGetConnectionStringInAppSettings()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringInAppSettings();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в секции appSettings конфигурационного файла приложения, 
        /// не указано ни имя строки соединения, ни сама строка соединения.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        public void GetConnectionStringInAppSettingsFail()
        {
            // Не задаем ни имени строки соединения, ни саму строку соединения.
            CreateApplicationConfigurationShims();

            // Не задаем аппендеру имя строки соединения.
            // Он должен попытаться взять строку прямо из секции appSettings конфигурационного файла.
            var appender = new CustomAdoNetAppender();
            string connectionString = appender.ConnectionString;
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в секции appSettings конфигурационного файла приложения, 
        /// не указано ни имя строки соединения, ни сама строка соединения.
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WinGetConnectionStringInAppSettingsFail()
        {
            using (ShimsContext.Create())
            {
                GetConnectionStringInAppSettingsFail();
            }
        }

        /// <summary>
        /// Аппендер должен выкинуть <see cref="ConfigurationErrorsException"/>, 
        /// если в секции appSettings конфигурационного файла приложения, 
        /// не указано ни имя строки соединения, ни сама строка соединения.
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void WebGetConnectionStringInAppSettingsFail()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetConnectionStringInAppSettingsFail();
            }
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        public void GetDecryptedConnectionStringByNameInAppender()
        {
            // Задаем зашифрованные строки соединения в секции connectionStrings
            // с настройкой в секции appSettings, указывающей на необходимость расшифровки строки при её получении.
            CreateApplicationConfigurationShims(null, null, true.ToString().ToLower(), true);

            var appender = new CustomAdoNetAppender { ConnectionStringName = ApplicationConnectionStringName };
            Assert.IsTrue(appender.ConnectionString == ApplicationConnectionString);
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        public void WinGetDecryptedConnectionStringByNameInAppender()
        {
            using (ShimsContext.Create())
            {
                GetDecryptedConnectionStringByNameInAppender();
            }
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        public void WebGetDecryptedConnectionStringByNameInAppender()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetDecryptedConnectionStringByNameInAppender();
            }
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        public void GetDecryptedConnectionStringInAppSettings()
        {
            // Не задаем никакого имени строки соединения, но задаем зашифрованную строку соединения прямо в секции appSettings
            // с настройкой, указывающей на необходимость расшифровки строки при её получении.
            CreateApplicationConfigurationShims(null, ApplicationConnectionString, true.ToString().ToLower(), true);

            // Не задаем аппендеру имя строки соединения.
            // Он должен взять строку прямо из секции appSettings конфигурационного файла и расшифровать её.
            var appender = new CustomAdoNetAppender();
            Assert.IsTrue(appender.ConnectionString == ApplicationConnectionString);
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        /// <remarks>Тест выполняется для win-окружения.</remarks>
        [TestMethod]
        public void WinGetDecryptedConnectionStringInAppSettings()
        {
            using (ShimsContext.Create())
            {
                GetDecryptedConnectionStringInAppSettings();
            }
        }

        /// <summary>
        /// Аппендер должен получать расшифрованную строку соединения по переданному ему имени,
        /// если в секции appSettings была указана настройка 
        /// <see cref="CustomAdoNetAppender.AppSettingConnectionStringIsEncrypted"/> со значением "true".
        /// </summary>
        /// <remarks>Тест выполняется для web-окружения.</remarks>
        [TestMethod]
        public void WebGetDecryptedConnectionStringInAppSettings()
        {
            using (ShimsContext.Create())
            {
                EmulateWebEnvironment();
                GetDecryptedConnectionStringInAppSettings();
            }
        }
    }
}
