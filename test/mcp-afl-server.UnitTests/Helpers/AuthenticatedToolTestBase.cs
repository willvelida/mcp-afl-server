using mcp_afl_server.Services;
using Microsoft.Graph.Models;
using Moq;
using RichardSzalay.MockHttp;

namespace mcp_afl_server.UnitTests.Helpers
{
    public abstract class AuthenticatedToolTestBase : IDisposable
    {
        protected readonly MockHttpMessageHandler MockHttpHandler;
        protected readonly HttpClient HttpClient;
        protected readonly Mock<IAuthenticationService> MockAuthenticationService;

        protected AuthenticatedToolTestBase()
        {
            (HttpClient, MockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            MockAuthenticationService = new Mock<IAuthenticationService>();
            
            // Setup default authenticated behavior
            SetupAuthenticatedUser();
        }

        protected void SetupAuthenticatedUser(string userId = "test-user-id", string userEmail = "test.user@example.com")
        {
            var testUser = new User
            {
                Id = userId,
                DisplayName = "Test User",
                UserPrincipalName = userEmail,
                Mail = userEmail
            };

            MockAuthenticationService.Setup(x => x.GetCurrentUserAsync())
                .ReturnsAsync(testUser);
            
            MockAuthenticationService.Setup(x => x.IsAuthenticated())
                .Returns(true);
        }

        protected void SetupUnauthenticatedUser()
        {
            MockAuthenticationService.Setup(x => x.GetCurrentUserAsync())
                .ThrowsAsync(new UnauthorizedAccessException("Authentication failed"));
            
            MockAuthenticationService.Setup(x => x.IsAuthenticated())
                .Returns(false);
        }

        protected User CreateTestUser(string userId = "test-user-id")
        {
            return new User
            {
                Id = userId,
                DisplayName = "Test User",
                UserPrincipalName = "test.user@example.com"
            };
        }

        public virtual void Dispose()
        {
            HttpClient?.Dispose();
            MockHttpHandler?.Dispose();
        }
    }
}