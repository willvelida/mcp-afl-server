using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using RichardSzalay.MockHttp;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class LadderToolsTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger<LadderTools>> _mockLogger;

        public LadderToolsTests() : base()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<LadderTools>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetProjectedLadderByRoundAndYear_ValidParameters_ReturnsLadderData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testLadderData = new[] 
            { 
                new { 
                    team = "Adelaide", 
                    teamid = 1, 
                    wins = "10", 
                    rank = 1, 
                    swarms = new[] { "swarm1" }, 
                    dummy = 0, 
                    year = 2024, 
                    round = 5, 
                    sourceid = 1, 
                    mean_rank = "1.0", 
                    source = "test-source",
                    updated = "2024-01-01" 
                },
                new { 
                    team = "Brisbane Lions", 
                    teamid = 2, 
                    wins = "8", 
                    rank = 2, 
                    swarms = new[] { "swarm1" }, 
                    dummy = 0, 
                    year = 2024, 
                    round = 5, 
                    sourceid = 1, 
                    mean_rank = "2.0", 
                    source = "test-source",
                    updated = "2024-01-01" 
                }
            };
            var expectedJson = JsonSerializer.Serialize(new { ladder = testLadderData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYearBySource_ValidParameters_ReturnsLadderData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const string source = "test-source";
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
            
            var testLadderData = new[] 
            { 
                new { 
                    team = "Adelaide", 
                    teamid = 1, 
                    wins = "10", 
                    rank = 1, 
                    swarms = new[] { "swarm1" }, 
                    dummy = 0, 
                    year = 2024, 
                    round = 5, 
                    sourceid = 1, 
                    mean_rank = "1.0", 
                    source = "test-source",
                    updated = "2024-01-01" 
                }
            };
            var expectedJson = JsonSerializer.Serialize(new { ladder = testLadderData });
            
            MockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round};source={source}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await ladderTools.GetProjectedLadderByRoundAndYearBySource(round, year, source);

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
        public async Task GetProjectedLadderByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Arrange
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Theory]
        [InlineData(2024, 5, "")]     // Empty source
        [InlineData(2024, 5, "   ")]  // Whitespace source  
        [InlineData(1800, 5, "test")] // Invalid year
        [InlineData(2024, 50, "test")]// Invalid round
        public async Task GetProjectedLadderByRoundAndYearBySource_InvalidParameters_ReturnsEmptyList(int year, int round, string source)
        {
            // Arrange
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act
            var result = await ladderTools.GetProjectedLadderByRoundAndYearBySource(round, year, source);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion

        #region Authentication Failure Tests

        [Fact]
        public async Task GetProjectedLadderByRoundAndYear_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            SetupUnauthenticatedUser();
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => ladderTools.GetProjectedLadderByRoundAndYear(round, year));
            exception.Should().NotBeNull();
            
            // Verify authentication was attempted
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYearBySource_Unauthenticated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const string source = "test-source";
            SetupUnauthenticatedUser();
            var ladderTools = new LadderTools(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => ladderTools.GetProjectedLadderByRoundAndYearBySource(round, year, source));
            exception.Should().NotBeNull();
            
            // Verify authentication was attempted
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        #endregion
    }
}