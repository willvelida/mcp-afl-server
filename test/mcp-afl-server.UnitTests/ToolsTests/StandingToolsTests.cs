using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class StandingToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<StandingsTools>> _mockLogger;

        public StandingToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<StandingsTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetCurrentStandings_ValidRequest_ReturnsStandingsData()
        {
            // Arrange
            var standingsTools = new StandingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testStandingsData = new[] 
            { 
                new { team = "Test Team 1", position = 1, points = 44, percentage = 125.5 },
                new { team = "Test Team 2", position = 2, points = 40, percentage = 115.2 }
            };
            var expectedJson = JsonSerializer.Serialize(new { standings = testStandingsData });
            
            MockHttpHandler
                .When("https://api.squiggle.com.au/?q=standings")
                .Respond("application/json", expectedJson);

            // Act
            var result = await standingsTools.GetCurrentStandings();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetStandingsByRoundAndYear_ValidParameters_ReturnsStandingsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var standingsTools = new StandingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testStandingsData = new[] 
            { 
                new { team = "Test Team 1", position = 1, points = 20, percentage = 125.5 }
            };
            var expectedJson = JsonSerializer.Serialize(new { standings = testStandingsData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=standings;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await standingsTools.GetStandingsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData(1800, 5)]  // Invalid year
        [InlineData(2024, 50)] // Invalid round
        [InlineData(2024, 0)]  // Invalid round
        [InlineData(2024, -1)] // Invalid round
        public async Task GetStandingsByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Arrange
            var standingsTools = new StandingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await standingsTools.GetStandingsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetCurrentStandings_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            SetupUnauthenticatedUser();
            var standingsTools = new StandingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => standingsTools.GetCurrentStandings());
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}