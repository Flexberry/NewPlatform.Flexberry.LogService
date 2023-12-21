namespace NewPlatform.Flexberry.LogService.Tests
{
    using ICSSoft.STORMNET;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Тесты <see cref="LogService" />.
    /// </summary>
    public class LogServiceTest
    {
        /// <summary>
        /// Test for <see cref="LogService.LogError(object)" /> method.
        /// </summary>
        [Fact]
        public void LogErrorTest()
        {
            LogService.LogError("Bums");
        }

        /// <summary>
        /// Test for reading connection string from appsettings.json and App.config.
        /// </summary>
        [Fact]
        public void ReadConnectionStringTest()
        {
            var adoNetAppender = LogService.Log.Logger.Repository.GetAppenders().First(x => x is CustomAdoNetAppender) as CustomAdoNetAppender;
            var connStr = adoNetAppender.GetConnectionStringFromConfiguration();
            Assert.NotNull(connStr);
#if NETCOREAPP
            Assert.DoesNotContain("SERVER=app.config.string", connStr);
#else
            Assert.Contains("SERVER=app.config.string", connStr);
#endif
        }
    }
}
