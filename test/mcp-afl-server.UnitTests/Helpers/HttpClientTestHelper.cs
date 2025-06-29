using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.Helpers
{
    public static class HttpClientTestHelper
    {
        public static (HttpClient httpClient, MockHttpMessageHandler mockHandler) CreateMockHttpClient()
        {
            var mockHandler = new MockHttpMessageHandler();
            var httpClient = mockHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://api.squiggle.com.au/");
            return (httpClient, mockHandler);
        }

        public static Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        public static void VerifyLogMessage<T>(Mock<ILogger<T>> mockLogger, LogLevel level, string message)
        {
            mockLogger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }
}