using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class SourcesToolsTests : IDisposable
    {
        private readonly Mock<ILogger<SourcesTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly SourcesTools _sourcesTools;

        public SourcesToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<SourcesTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _sourcesTools = new SourcesTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetSources_ValidRequest_ReturnsSourcesData()
        {
            // Arrange
            var expectedJson = TestData.CreateSourcesResponseJson();
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=sources")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _sourcesTools.GetSources();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetSources_ApiReturnsError_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=sources")
                .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await _sourcesTools.GetSources();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        [Fact]
        public async Task GetSourceById_ValidSourceId_ReturnsSourceData()
        {
            // Arrange
            const string sourceId = "test-source";
            var expectedJson = TestData.CreateSourcesResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=sources;source={sourceId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _sourcesTools.GetSourceById(sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetSourceById_InvalidSourceId_ReturnsEmptyList(string invalidSourceId)
        {
            // Act
            var result = await _sourcesTools.GetSourceById(invalidSourceId!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Source ID cannot be null or empty");
        }

        [Fact]
        public async Task GetSourceById_SourceIdWithSpecialCharacters_EncodesCorrectly()
        {
            // Arrange
            const string sourceIdWithSpecialChars = "test source & more";
            var expectedJson = TestData.CreateSourcesResponseJson();
            
            // The URL should be encoded properly
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=sources;source=test%20source%20%26%20more")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _sourcesTools.GetSourceById(sourceIdWithSpecialChars);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetSources_MissingSourcesProperty_ReturnsEmptyList()
        {
            // Arrange
            var jsonWithoutSourcesProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=sources")
                .Respond("application/json", jsonWithoutSourcesProperty);

            // Act
            var result = await _sourcesTools.GetSources();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'sources' property found");
        }

        [Fact]
        public async Task GetSourceById_NetworkException_ReturnsEmptyList()
        {
            // Arrange
            const string sourceId = "test-source";
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=sources;source={sourceId}")
                .Throw(new HttpRequestException("Network failure"));

            // Act
            var result = await _sourcesTools.GetSourceById(sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "Network error");
        }

        [Fact]
        public async Task GetSources_Timeout_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=sources")
                .Throw(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _sourcesTools.GetSources();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "Timeout");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}