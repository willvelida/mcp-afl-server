using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class GameToolsTests : IDisposable
    {
        private readonly Mock<ILogger<GameTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly GameTools _gameTools;

        public GameToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<GameTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _gameTools = new GameTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetGameResult_ValidGameId_ReturnsGameData()
        {
            // Arrange
            const int gameId = 12345;
            var expectedJson = TestData.CreateGameResponseJson(gameId);
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=games;game={gameId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _gameTools.GetGameResult(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetGameResult_InvalidGameId_ReturnsEmptyList(int invalidGameId)
        {
            // Act
            var result = await _gameTools.GetGameResult(invalidGameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Invalid gameId parameter");
        }

        [Fact]
        public async Task GetRoundResultsByYear_ValidParameters_ReturnsGameData()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            var expectedJson = TestData.CreateGameResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=games;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _gameTools.GetRoundResultsByYear(year, round);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1800, 1)] // Invalid year - too old
        [InlineData(2030, 1)] // Invalid year - too far in future
        [InlineData(2024, 0)] // Invalid round - too low
        [InlineData(2024, 31)] // Invalid round - too high
        public async Task GetRoundResultsByYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Act
            var result = await _gameTools.GetRoundResultsByYear(year, round);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify warning was logged for invalid parameters
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}