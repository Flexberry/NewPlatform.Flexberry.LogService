namespace ICSSoft.STORMNET
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Security.Cryptography;
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
        /// <summary>
        /// Наименование настройки в секции appSettings, содержащей имя строки соединения из конфигурационной секции connectionStrings.
        /// </summary>
        public const string AppSettingConnectionStringName = "DefaultConnectionStringName";

        /// <summary>
        /// Наименование настройки в секции appSettings, содержащей строку соединения.
        /// </summary>
        public const string AppSettingConnectionString = "CustomizationStrings";

        /// <summary>
        /// Наименование настройки в секции appSettings, содержащей флаг, который показывает является ли строка соедиения зашифрованной.
        /// </summary>
        public const string AppSettingConnectionStringIsEncrypted = "Encrypted";

        /// <summary>
        /// Строка соединения с базой данных.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Получает или задает строку соединения с базой данных.
        /// </summary>
        /// <remarks>
        /// Переопределяет свойство базового класса, чтобы выполнить собственную логику получения строки соединения.
        /// Так как в базовом классе методы get и set не содержат логики, то это вполне безопасно.
        /// </remarks>
        public new string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    _connectionString = GetConnectionStringFromConfiguration();
                }

                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Осуществляет получение строки соединения из конфигурационного файла приложения.
        /// </summary>
        /// <returns>Полученная строка соединения.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Выбрасывается, если не удалось получить строку соединения из конфигурационного файла приложения.
        /// </exception>
        private string GetConnectionStringFromConfiguration()
        {
            // Конфигурационная секция appSettings.
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            // Конфигурационная секция connectionStrings.
            ConnectionStringSettingsCollection appConnectionStrings = ConfigurationManager.ConnectionStrings;

            // Имя строки соединения в конфигурационной секции connectionStrings.
            string connectionStringName = string.IsNullOrEmpty(ConnectionStringName)
                ? appSettings[AppSettingConnectionStringName]
                : ConnectionStringName;

            // Собственно строка соединения.
            string connectionString = null;
            if (string.IsNullOrEmpty(connectionStringName))
            {
                connectionString = appSettings[AppSettingConnectionString];
            }
            else
            {
                ConnectionStringSettings connectionStringSettings = appConnectionStrings[connectionStringName];
                if (connectionStringSettings != null)
                {
                    connectionString = connectionStringSettings.ConnectionString;
                }
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "Не удалось найти строку соединения в конфигурационном файле приложения. " +
                    "Удостоверьтесь, что в секции \"appSettings\" задана настройка \"DefaultConnectionStringName\"," +
                    " и ей соответствует корректная строка соединения в секции \"connectionStrings\", " +
                    "либо удостоверьтесь, что в секции \"appSettings\" корректно задана настройка \"CustomizationStrings\"");
            }

            // Если в конфигурационном файле указано, что строка соединения зашифрована, то нужно её расшифровать.
            string appSettingConnectionStringIsEncrypted = appSettings[AppSettingConnectionStringIsEncrypted];
            if (!string.IsNullOrEmpty(appSettingConnectionStringIsEncrypted))
            {
                appSettingConnectionStringIsEncrypted = appSettingConnectionStringIsEncrypted.ToLower();
            }

            bool connectionStringIsEncrypted = appSettingConnectionStringIsEncrypted == true.ToString().ToLower();
            if (connectionStringIsEncrypted)
            {
                connectionString = DecryptString(connectionString, true);
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
                Padding = PaddingMode.PKCS7
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
