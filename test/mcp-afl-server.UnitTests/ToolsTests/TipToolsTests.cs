using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class TipToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<TipsTools>> _mockLogger;

        public TipToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<TipsTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetTipsByRoundAndYear_ValidParameters_ReturnsTipsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testTipsData = new[] 
            { 
                new { game = 1, tipster = "Test Tipster 1", tip = "Test Team 1" },
                new { game = 2, tipster = "Test Tipster 2", tip = "Test Team 2" }
            };
            var expectedJson = JsonSerializer.Serialize(new { tips = testTipsData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await tipsTools.GetTipsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTipsByGame_ValidGameId_ReturnsTipsData()
        {
            // Arrange
            const int gameId = 12345;
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testTipsData = new[] 
            { 
                new { game = gameId, tipster = "Test Tipster 1", tip = "Test Team 1" }
            };
            var expectedJson = JsonSerializer.Serialize(new { tips = testTipsData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=tips;game={gameId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await tipsTools.GetTipsByGame(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFutureTips_ValidRequest_ReturnsTipsData()
        {
            // Arrange
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testTipsData = new[] 
            { 
                new { game = 1, tipster = "Test Tipster 1", tip = "Test Team 1", complete = 0 },
                new { game = 2, tipster = "Test Tipster 2", tip = "Test Team 2", complete = 50 }
            };
            var expectedJson = JsonSerializer.Serialize(new { tips = testTipsData });
            
            MockHttpHandler
                .When("https://api.squiggle.com.au/?q=tips;complete=!100")
                .Respond("application/json", expectedJson);

            // Act
            var result = await tipsTools.GetFutureTips();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData(1800, 5)]  // Invalid year
        [InlineData(2024, 50)] // Invalid round
        [InlineData(2024, 0)]  // Invalid round
        [InlineData(2024, -1)] // Invalid round
        public async Task GetTipsByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Arrange
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await tipsTools.GetTipsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]   // Invalid game ID
        [InlineData(-1)]  // Invalid game ID
        public async Task GetTipsByGame_InvalidGameId_ReturnsEmptyList(int gameId)
        {
            // Arrange
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await tipsTools.GetTipsByGame(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetTipsByRoundAndYear_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            SetupUnauthenticatedUser();
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => tipsTools.GetTipsByRoundAndYear(5, 2024));
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFutureTips_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            SetupUnauthenticatedUser();
            var tipsTools = new TipsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => tipsTools.GetFutureTips());
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}