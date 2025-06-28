using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class TipsToolsTests : IDisposable
    {
        private readonly Mock<ILogger<TipsTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly TipsTools _tipsTools;

        public TipsToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<TipsTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _tipsTools = new TipsTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTipsByRoundAndYear_ValidParameters_ReturnsTipsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var expectedJson = TestData.CreateTipsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _tipsTools.GetTipsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1800, 1)] // Invalid year - too old
        [InlineData(2030, 1)] // Invalid year - too far in future
        [InlineData(2024, 0)] // Invalid round - too low
        [InlineData(2024, 31)] // Invalid round - too high
        public async Task GetTipsByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Act
            var result = await _tipsTools.GetTipsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Invalid");
        }

        [Fact]
        public async Task GetTipsByGame_ValidGameId_ReturnsTipsData()
        {
            // Arrange
            const int gameId = 12345;
            var expectedJson = TestData.CreateTipsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;game={gameId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _tipsTools.GetTipsByGame(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetTipsByGame_InvalidGameId_ReturnsEmptyList(int invalidGameId)
        {
            // Act
            var result = await _tipsTools.GetTipsByGame(invalidGameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Game ID must be a positive integer");
        }

        [Fact]
        public async Task GetFutureTips_ValidRequest_ReturnsTipsData()
        {
            // Arrange
            var expectedJson = TestData.CreateTipsResponseJson();
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=tips;complete=!100")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _tipsTools.GetFutureTips();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetFutureTips_ApiReturnsError_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=tips;complete=!100")
                .Respond(HttpStatusCode.ServiceUnavailable);

            // Act
            var result = await _tipsTools.GetFutureTips();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        [Fact]
        public async Task GetTipsByRoundAndYear_MissingTipsProperty_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            var jsonWithoutTipsProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;year={year};round={round}")
                .Respond("application/json", jsonWithoutTipsProperty);

            // Act
            var result = await _tipsTools.GetTipsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'tips' property found");
        }

        [Fact]
        public async Task GetTipsByGame_NetworkException_ReturnsEmptyList()
        {
            // Arrange
            const int gameId = 12345;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;game={gameId}")
                .Throw(new HttpRequestException("Network failure"));

            // Act
            var result = await _tipsTools.GetTipsByGame(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "Network error");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}