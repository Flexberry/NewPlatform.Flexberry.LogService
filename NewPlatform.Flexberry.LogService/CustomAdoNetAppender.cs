namespace ICSSoft.STORMNET
{
#if NETSTANDARD2_0_OR_GREATER
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
#endif
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
#if NETSTANDARD2_0_OR_GREATER
    using System.IO;
#endif
    using System.Security.Cryptography;
    using System.Security.Policy;
    using System.Text;

    using log4net.Appender;

    /// <summary>
    /// Log4net аппендер для записи сообщений лога в БД.
    /// </summary>
    /// <remarks>
    /// Наследуется от стандартного <see cref="AdoNetAppender"/> сохраняя всю функциональность родителя,
    /// но переопределяет логику получения строки соединения.
    /// </remarks>
    public class CustomAdoNetAppender : AdoNetAppender
    {

#if NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Конструктор, который загружает настройки в appSettingsJson при первом обращении к CustomAdoNetAppender.
        /// </summary>
        static CustomAdoNetAppender()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            appSettingsJson = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json", true, true)
                .Build();
        }
#endif

        /// <summary>
        /// Наименование настройки в секции appSettings, содержащей имя строки соединения из конфигурационной секции connectionStrings.
        /// </summary>
        public const string DefaultConnectionStringNameSetting = "DefaultConnectionStringName";

        /// <summary>
        /// Наименование настройки в секции appSettings, содержащей строку соединения.
        /// </summary>
        public const string AppSettingConnectionString = "CustomizationStrings";

        /// <summary>
        /// Настройка - является ли строка соедиения зашифрованной.
        /// </summary>
        public bool IsConnectionStringEncrypted => bool.Parse(AppSettings["Encrypted"] ?? "false");

        /// <summary>
        /// Наименование строки соединения. Улучшенный вариант, позволяющий задать DefaultConnectionStringName и использовать параметр "CustomizationStrings" если ничего не задано.
        /// </summary>
        public string CustomConnectionStringName => ConnectionStringName.NullIfEmpty() ??
                                                    AppSettings[DefaultConnectionStringNameSetting].NullIfEmpty() ??
                                                    AppSettingConnectionString.NullIfEmpty();

#if NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Параметры из файла конфигурации appsettings.json.
        /// </summary>
        private static IConfiguration appSettingsJson;
#endif

        /// <summary>
        /// Конфигурационная секция appSettings из App.config.
        /// </summary>
        private NameValueCollection AppSettings => System.Configuration.ConfigurationManager.AppSettings;

        /// <summary>
        /// Осуществляет получение строки соединения из конфигурационного файла приложения.
        /// </summary>
        /// <returns>Полученная строка соединения.</returns>
        public string GetConnectionStringFromConfiguration()
        {
            var connectionString = GetConnectionString(CustomConnectionStringName);

            // Если в конфигурационном файле указано, что строка соединения зашифрована, то нужно её расшифровать.
            if (IsConnectionStringEncrypted)
            {
                connectionString = DecryptString(connectionString, true);
            }

            return connectionString;
        }

        /// <inheritdoc/>
        protected override string ResolveConnectionString(out string connectionStringContext)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                connectionStringContext = "ConnectionString";
                return ConnectionString;
            }

            connectionStringContext = CustomConnectionStringName;
            return GetConnectionStringFromConfiguration();
        }

        /// <summary>
        /// Получить строку соединения.
        /// </summary>
        /// <param name="connectionStringName">Название строки в разделе ConnectionStrings.</param>
        /// <returns>Строка соединения.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Выбрасывается, если не удалось получить строку соединения из конфигурационного файла приложения.
        /// </exception>
        private string GetConnectionString(string connectionStringName)
        {
            string connectionString = null;

#if NETSTANDARD2_0_OR_GREATER
            connectionString = appSettingsJson.GetConnectionString(connectionStringName);
            if (connectionString != null)
            {
                return connectionString;
            }
#endif

            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString;
            if (connectionString == null)
            {
                connectionString = AppSettings[connectionStringName];
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "Не удалось найти строку соединения в конфигурационном файле приложения. " +
                    "Удостоверьтесь, что в AdoNetAppender передаётся параметр \"ConnectionStringName\". Если он не указан, " +
                    "удостоверьтесь, что в секции \"appSettings\" задана настройка \"DefaultConnectionStringName\"," +
                    " и ей соответствует корректная строка соединения в секции \"connectionStrings\", " +
                    "либо удостоверьтесь, что в секции \"appSettings\" корректно задана настройка \"CustomizationStrings\"");
            }

            return connectionString;
        }

        /// <summary>
        /// Осуществляет шифрование переданной строки.
        /// </summary>
        /// <param name="originalString">Строка, которую необходимо зашифровать.</param>
        /// <param name="useHashing">Флаг: использовать ли хэш в качестве ключа.</param>
        /// <returns>Зашифрованная строка.</returns>
        public static string EncryptString(string originalString, bool useHashing)
        {
            byte[] originalBytes = Encoding.UTF8.GetBytes(originalString);
            byte[] encryptedBytes;

            using (TripleDESCryptoServiceProvider cryptographicServiceProvider = GetCryptographicServiceProvider(useHashing))
            {
                ICryptoTransform encryptor = cryptographicServiceProvider.CreateEncryptor();
                encryptedBytes = encryptor.TransformFinalBlock(originalBytes, 0, originalBytes.Length);
                cryptographicServiceProvider.Clear();
            }

            return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
        }

        /// <summary>
        /// Осуществляет расшифровку переданной строки.
        /// </summary>
        /// <param name="encryptedString">Строка, которую необходимо расшифровать.</param>
        /// <param name="useHashing">Флаг: использовать ли хэш в качестве ключа.</param>
        /// <returns>Расшифрованная строка.</returns>
        public static string DecryptString(string encryptedString, bool useHashing)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedString);
            byte[] decryptedBytes;

            using (TripleDESCryptoServiceProvider cryptographicServiceProvider = GetCryptographicServiceProvider(useHashing))
            {
                ICryptoTransform decryptor = cryptographicServiceProvider.CreateDecryptor();
                decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                cryptographicServiceProvider.Clear();
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Осуществляет получение провайдера криптографической службы.
        /// </summary>
        /// <param name="useHashing">Флаг: использовать ли хэш в качестве ключа.</param>
        /// <returns>Провайдер криптографической службы.</returns>
        private static TripleDESCryptoServiceProvider GetCryptographicServiceProvider(bool useHashing)
        {
            var cryptographicServiceProvider = new TripleDESCryptoServiceProvider
            {
                Key = GetCryptographicKeyBytes(useHashing),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
            };

            return cryptographicServiceProvider;
        }

        /// <summary>
        /// Осуществляет получение массива байтов криптографического ключа.
        /// </summary>
        /// <param name="useHashing">Флаг: использовать ли хэш, в качестве ключа.</param>
        /// <returns>Полученный массив байтов криптографического ключа.</returns>
        private static byte[] GetCryptographicKeyBytes(bool useHashing)
        {
            string key = "SecurityKey";
            byte[] keyBytes;

            if (useHashing)
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    md5.Clear();
                }
            }
            else
            {
                keyBytes = Encoding.UTF8.GetBytes(key);
            }

            return keyBytes;
        }
    }
}
