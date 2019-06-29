namespace NewPlatform.Flexberry.LogService.Tests
{
    using Xunit;

    public class LogServiceTest
    {
        /// <summary>
        /// Test for <see cref="ICSSoft.STORMNET.LogService.LogError"/> method.
        /// </summary>
        [Fact]
        void LogErrorTest()
        {
            ICSSoft.STORMNET.LogService.LogError("Bums");
        }
    }
}
