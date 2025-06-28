using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class PowerRankingsToolsTests : IDisposable
    {
        private readonly Mock<ILogger<PowerRankingsTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly PowerRankingsTools _powerRankingsTools;

        public PowerRankingsToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<PowerRankingsTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _powerRankingsTools = new PowerRankingsTools(_httpClient, _mockLogger.Object);
        }

        #region GetPowerRankingByRoundAndYear Tests

        [Theory]
        [InlineData(2022)] // First valid year for power rankings
        [InlineData(2024)] // Current valid year
        [InlineData(2025)] // Future valid year
        public async Task GetPowerRankingByRoundAndYear_ValidYear_ReturnsData(int validYear)
        {
            // Arrange
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={validYear};round=1")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, validYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // Based on test data
            
            // Verify successful operation was logged
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Information, "Successfully retrieved");
        }

        [Theory]
        [InlineData(2021)] // Before power rankings started
        [InlineData(2020)] // Way before power rankings
        [InlineData(1897)] // First AFL year but no power rankings
        [InlineData(2000)] // Early 2000s - no power rankings
        public async Task GetPowerRankingByRoundAndYear_YearBeforePowerRankings_ReturnsEmptyList(int invalidYear)
        {
            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, invalidYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify specific power ranking error message
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, 
                "Year must be 2022 or later (power rankings not available before 2022)");
        }

        [Theory]
        [InlineData(2030)] // Too far in future
        [InlineData(2050)] // Way too far in future
        public async Task GetPowerRankingByRoundAndYear_YearTooFarInFuture_ReturnsEmptyList(int invalidYear)
        {
            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, invalidYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Should still get the power rankings specific error (since it's checked first)
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, 
                "Year must be 2022 or later");
        }

        [Theory]
        [InlineData(0)]  // Round too low
        [InlineData(-1)] // Negative round
        [InlineData(31)] // Round too high
        [InlineData(50)] // Way too high
        public async Task GetPowerRankingByRoundAndYear_InvalidRound_ReturnsEmptyList(int invalidRound)
        {
            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(invalidRound, 2024);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify round validation error
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Round must be between 1 and 30");
        }

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_ValidParameters_CorrectEndpointCalled()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            var expectedUrl = $"https://api.squiggle.com.au/?q=power;year={year};round={round}";
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            
            // Verify the correct endpoint was called (implicit through mock setup)
        }

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_ApiError_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=power;year=2024;round=1")
                .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, 2024);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        #endregion

        #region GetPowerRankingByRoundYearAndSource Tests

        [Fact]
        public async Task GetPowerRankingByRoundYearAndSource_ValidParameters_ReturnsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            const int sourceId = 5;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};source={sourceId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundYearAndSource(round, year, sourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(0)]  // Invalid source ID
        [InlineData(-1)] // Negative source ID
        [InlineData(-10)] // Very negative source ID
        public async Task GetPowerRankingByRoundYearAndSource_InvalidSourceId_ReturnsEmptyList(int invalidSourceId)
        {
            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundYearAndSource(1, 2024, invalidSourceId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify source ID validation error
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Source ID must be a positive integer");
        }

        [Fact]
        public async Task GetPowerRankingByRoundYearAndSource_InvalidYearAndValidSource_ReturnsEmptyList()
        {
            // Act - Test year validation takes precedence
            var result = await _powerRankingsTools.GetPowerRankingByRoundYearAndSource(1, 2021, 5);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Should get the year validation error (checked first)
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, 
                "Year must be 2022 or later");
        }

        #endregion

        #region GetTeamPowerRankingByRoundAndYear Tests

        [Fact]
        public async Task GetTeamPowerRankingByRoundAndYear_ValidParameters_ReturnsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            const int teamId = 7;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};team={teamId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetTeamPowerRankingByRoundAndYear(round, year, teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(0)]  // Invalid team ID
        [InlineData(-1)] // Negative team ID
        [InlineData(-5)] // Very negative team ID
        public async Task GetTeamPowerRankingByRoundAndYear_InvalidTeamId_ReturnsEmptyList(int invalidTeamId)
        {
            // Act
            var result = await _powerRankingsTools.GetTeamPowerRankingByRoundAndYear(1, 2024, invalidTeamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify team ID validation error
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Team ID must be a positive integer");
        }

        [Fact]
        public async Task GetTeamPowerRankingByRoundAndYear_ValidTeamHighRound_ReturnsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 23; // High but valid round
            const int teamId = 1;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};team={teamId}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetTeamPowerRankingByRoundAndYear(round, year, teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_MissingPowerProperty_ReturnsEmptyList()
        {
            // Arrange
            var jsonWithoutPowerProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=power;year=2024;round=1")
                .Respond("application/json", jsonWithoutPowerProperty);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, 2024);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'power' property found");
        }

        [Fact]
        public async Task GetTeamPowerRankingByRoundAndYear_NetworkException_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            const int teamId = 1;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={year};round={round};team={teamId}")
                .Throw(new HttpRequestException("Network failure"));

            // Act
            var result = await _powerRankingsTools.GetTeamPowerRankingByRoundAndYear(round, year, teamId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "Network error");
        }

        [Fact]
        public async Task GetPowerRankingByRoundYearAndSource_Timeout_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=power;year=2024;round=1;source=1")
                .Throw(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundYearAndSource(1, 2024, 1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "Timeout");
        }

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_InvalidJson_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=power;year=2024;round=1")
                .Respond("application/json", "invalid json");

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, 2024);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "JSON parsing error");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_CurrentYear_ReturnsData()
        {
            // Arrange
            var currentYear = DateTime.Now.Year;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={currentYear};round=1")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, currentYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPowerRankingByRoundAndYear_NextYear_ReturnsData()
        {
            // Arrange
            var nextYear = DateTime.Now.Year + 1;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year={nextYear};round=1")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetPowerRankingByRoundAndYear(1, nextYear);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetTeamPowerRankingByRoundAndYear_MaxValidRound_ReturnsData()
        {
            // Arrange
            const int maxRound = 30;
            var expectedJson = TestData.CreatePowerRankingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=power;year=2024;round={maxRound};team=1")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _powerRankingsTools.GetTeamPowerRankingByRoundAndYear(maxRound, 2024, 1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}