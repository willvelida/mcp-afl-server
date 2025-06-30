using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class SourcesToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<SourcesTools>> _mockLogger;

        public SourcesToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<SourcesTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetSources_ValidRequest_ReturnsSourcesData()
        {
            // Arrange
            var sourcesTools = new SourcesTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testSourcesData = new[] 
            { 
                new { id = 1, name = "Test Source 1", url = "https://example1.com" },
                new { id = 2, name = "Test Source 2", url = "https://example2.com" }
            };
            var expectedJson = JsonSerializer.Serialize(new { sources = testSourcesData });
            
            MockHttpHandler
                .When("https://api.squiggle.com.au/?q=sources")
                .Respond("application/json", expectedJson);

            // Act
            var result = await sourcesTools.GetSources();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSourceById_ValidSourceId_ReturnsSourceData()
        {
            // Arrange
            const string sourceId = "test-source-123";
            var sourcesTools = new SourcesTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testSourceData = new[] 
            { 
                new { id = sourceId, name = "Test Source", url = "https://example.com" }
            };
            var expectedJson = JsonSerializer.Serialize(new { sources = testSourceData });
            var escapedSourceId = Uri.EscapeDataString(sourceId);
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=sources;source={escapedSourceId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await sourcesTools.GetSourceById(sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData("")]      // Empty string
        [InlineData("   ")]   // Whitespace
        public async Task GetSourceById_InvalidSourceId_ReturnsEmptyList(string sourceId)
        {
            // Arrange
            var sourcesTools = new SourcesTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await sourcesTools.GetSourceById(sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetSources_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            SetupUnauthenticatedUser();
            var sourcesTools = new SourcesTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sourcesTools.GetSources());
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}