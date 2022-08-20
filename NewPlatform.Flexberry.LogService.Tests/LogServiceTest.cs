namespace NewPlatform.Flexberry.LogService.Tests
{
    using ICSSoft.STORMNET;

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
    }
}
