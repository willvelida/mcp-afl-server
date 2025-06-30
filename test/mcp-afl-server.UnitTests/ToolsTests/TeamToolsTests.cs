using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class TeamToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<TeamTools>> _mockLogger;

        public TeamToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<TeamTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetTeamInfo_ValidTeamId_ReturnsTeamData()
        {
            // Arrange
            const int teamId = 123;
            var teamTools = new TeamTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testTeamData = new[] 
            { 
                new { id = teamId, name = "Test Team", abbrev = "TT", logo = "test-logo.png" }
            };
            var expectedJson = JsonSerializer.Serialize(new { teams = testTeamData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;team={teamId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await teamTools.GetTeamInfo(teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTeamsBySeason_ValidYear_ReturnsTeamsData()
        {
            // Arrange
            const int year = 2024;
            var teamTools = new TeamTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testTeamsData = new[] 
            { 
                new { id = 1, name = "Test Team 1", abbrev = "TT1", logo = "test-logo1.png" },
                new { id = 2, name = "Test Team 2", abbrev = "TT2", logo = "test-logo2.png" }
            };
            var expectedJson = JsonSerializer.Serialize(new { teams = testTeamsData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;year={year}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await teamTools.GetTeamsBySeason(year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Parameter Validation Tests

        [Theory]
        [InlineData(0)]   // Invalid team ID
        [InlineData(-1)]  // Invalid team ID
        public async Task GetTeamInfo_InvalidTeamId_ReturnsEmptyList(int teamId)
        {
            // Arrange
            var teamTools = new TeamTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await teamTools.GetTeamInfo(teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1800)]  // Invalid year
        [InlineData(2030)]  // Invalid year (too far future)
        public async Task GetTeamsBySeason_InvalidYear_ReturnsEmptyList(int year)
        {
            // Arrange
            var teamTools = new TeamTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await teamTools.GetTeamsBySeason(year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetTeamInfo_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            const int teamId = 123;
            SetupUnauthenticatedUser();
            var teamTools = new TeamTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => teamTools.GetTeamInfo(teamId));
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}