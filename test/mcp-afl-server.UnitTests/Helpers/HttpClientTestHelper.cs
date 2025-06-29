using mcp_afl_server.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using Microsoft.Graph.Models;
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

        public static Mock<IHttpContextAccessor> CreateMockHttpContextAccessor(string? bearerToken = "test-token")
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            var mockHeaders = new Mock<IHeaderDictionary>();

            // Setup the authorization header
            if (!string.IsNullOrEmpty(bearerToken))
            {
                mockHeaders.Setup(h => h["Authorization"])
                    .Returns(new StringValues($"Bearer {bearerToken}"));
            }

            mockRequest.Setup(r => r.Headers).Returns(mockHeaders.Object);
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            return mockHttpContextAccessor;
        }

        public static Mock<IOptions<AzureAdOptions>> CreateMockAzureAdOptions()
        {
            var azureAdOptions = new AzureAdOptions
            {
                TenantId = "test-tenant-id",
                ClientId = "test-client-id",
                ManagedIdentityClientId = "test-managed-identity-id"
            };

            var mockOptions = new Mock<IOptions<AzureAdOptions>>();
            mockOptions.Setup(o => o.Value).Returns(azureAdOptions);
            return mockOptions;
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