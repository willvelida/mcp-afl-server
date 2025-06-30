using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class GameToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<GameTools>> _mockLogger;

        public GameToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<GameTools>();
        }

        #region Happy Path Tests (Authenticated)

        [Fact]
        public async Task GetGameResult_ValidGameId_ReturnsGameData()
        {
            // Arrange
            const int gameId = 12345;
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testGameData = new[] 
            { 
                new { id = gameId, home_team = "Test Team 1", away_team = "Test Team 2", score = "100-85" } 
            };
            var expectedJson = JsonSerializer.Serialize(new { games = testGameData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=games;game={gameId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await gameTools.GetGameResult(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            
            // Verify authentication was called
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRoundResultsByYear_ValidParameters_ReturnsRoundData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testGameData = new[] 
            { 
                new { id = 1, year = year, round = round, home_team = "Test Team 1", away_team = "Test Team 2" },
                new { id = 2, year = year, round = round, home_team = "Test Team 3", away_team = "Test Team 4" }
            };
            var expectedJson = JsonSerializer.Serialize(new { games = testGameData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=games;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await gameTools.GetRoundResultsByYear(year, round);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            
            // Verify authentication was called
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetGameResult_InvalidGameId_ReturnsEmptyList(int invalidGameId)
        {
            // Arrange
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await gameTools.GetGameResult(invalidGameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify authentication was still called (happens before validation)
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1800, 5)]  // Invalid year
        [InlineData(2024, 50)] // Invalid round
        public async Task GetRoundResultsByYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Arrange
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await gameTools.GetRoundResultsByYear(year, round);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify authentication was called
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetGameResult_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            const int gameId = 12345;
            SetupUnauthenticatedUser();
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => gameTools.GetGameResult(gameId));
            exception.Should().NotBeNull();
            
            // Verify authentication was attempted
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetRoundResultsByYear_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            SetupUnauthenticatedUser();
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => gameTools.GetRoundResultsByYear(year, round));
            exception.Should().NotBeNull();
            
            // Verify authentication was attempted
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region HTTP Error Tests

        [Fact]
        public async Task GetGameResult_ApiError_ReturnsEmptyList()
        {
            // Arrange
            const int gameId = 12345;
            var gameTools = new GameTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=games;game={gameId}")
                .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act
            var result = await gameTools.GetGameResult(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify authentication was called
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}