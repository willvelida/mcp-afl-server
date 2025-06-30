using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class PowerRankingToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<PowerRankingsTools>> _mockLogger;

        public PowerRankingToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<PowerRankingsTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_ValidParameters_ReturnsPowerData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testPowerData = new[] 
            { 
                new { team = "Test Team 1", ranking = 1, rating = 85.5 },
                new { team = "Test Team 2", ranking = 2, rating = 82.3 }
            };
            var expectedJson = JsonSerializer.Serialize(new { power = testPowerData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await powerRankingsTools.GetPowerRankingByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPowerRankingByRoundYearAndSource_ValidParameters_ReturnsPowerData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const int sourceId = 123;
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testPowerData = new[] 
            { 
                new { team = "Test Team 1", ranking = 1, rating = 85.5 }
            };
            var expectedJson = JsonSerializer.Serialize(new { power = testPowerData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};source={sourceId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await powerRankingsTools.GetPowerRankingByRoundYearAndSource(round, year, sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTeamPowerRankingByRoundAndYear_ValidParameters_ReturnsPowerData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const int teamId = 456;
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testPowerData = new[] 
            { 
                new { team = "Test Team 1", ranking = 1, rating = 85.5 }
            };
            var expectedJson = JsonSerializer.Serialize(new { power = testPowerData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};team={teamId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await powerRankingsTools.GetTeamPowerRankingByRoundAndYear(round, year, teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData(2021, 5)]  // Invalid year (before 2022)
        [InlineData(2024, 50)] // Invalid round
        [InlineData(2024, 0)]  // Invalid round
        public async Task GetPowerRankingByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Arrange
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await powerRankingsTools.GetPowerRankingByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Theory]
        [InlineData(2021, 5, 123)]  // Invalid year
        [InlineData(2024, 50, 123)] // Invalid round
        [InlineData(2024, 5, 0)]    // Invalid source ID
        [InlineData(2024, 5, -1)]   // Invalid source ID
        public async Task GetPowerRankingByRoundYearAndSource_InvalidParameters_ReturnsEmptyList(int year, int round, int sourceId)
        {
            // Arrange
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await powerRankingsTools.GetPowerRankingByRoundYearAndSource(round, year, sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            SetupUnauthenticatedUser();
            var powerRankingsTools = new PowerRankingsTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => powerRankingsTools.GetPowerRankingByRoundAndYear(5, 2024));
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}