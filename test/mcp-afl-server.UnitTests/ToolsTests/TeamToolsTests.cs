using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class TeamToolsTests : IDisposable
    {
        private readonly Mock<ILogger<TeamTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly TeamTools _teamTools;

        public TeamToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<TeamTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _teamTools = new TeamTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTeamInfo_ValidTeamId_ReturnsTeamData()
        {
            // Arrange
            const int teamId = 1;
            var expectedJson = TestData.CreateTeamResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;team={teamId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _teamTools.GetTeamInfo(teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetTeamInfo_InvalidTeamId_ReturnsEmptyList(int invalidTeamId)
        {
            // Act
            var result = await _teamTools.GetTeamInfo(invalidTeamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Team ID must be a positive integer");
        }

        [Fact]
        public async Task GetTeamInfo_ApiReturnsNotFound_ReturnsEmptyList()
        {
            // Arrange
            const int teamId = 999;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;team={teamId}")
                .Respond(HttpStatusCode.NotFound);

            // Act
            var result = await _teamTools.GetTeamInfo(teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        [Fact]
        public async Task GetTeamsBySeason_ValidYear_ReturnsTeamData()
        {
            // Arrange
            const int year = 2024;
            var expectedJson = TestData.CreateTeamResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;year={year}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _teamTools.GetTeamsBySeason(year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1800)] // Invalid year - too old
        [InlineData(2030)] // Invalid year - too far in future
        public async Task GetTeamsBySeason_InvalidYear_ReturnsEmptyList(int invalidYear)
        {
            // Act
            var result = await _teamTools.GetTeamsBySeason(invalidYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Year must be between 1897 and current year + 1");
        }

        [Fact]
        public async Task GetTeamsBySeason_MissingTeamsProperty_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            var jsonWithoutTeamsProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;year={year}")
                .Respond("application/json", jsonWithoutTeamsProperty);

            // Act
            var result = await _teamTools.GetTeamsBySeason(year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'teams' property found");
        }

        [Fact]
        public async Task GetTeamInfo_Timeout_ReturnsEmptyList()
        {
            // Arrange
            const int teamId = 1;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=teams;team={teamId}")
                .Throw(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _teamTools.GetTeamInfo(teamId);

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