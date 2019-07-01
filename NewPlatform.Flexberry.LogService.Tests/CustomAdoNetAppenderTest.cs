namespace LogService.Tests
{
    using ICSSoft.STORMNET;

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
    }
}
