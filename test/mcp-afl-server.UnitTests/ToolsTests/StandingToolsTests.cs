using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class StandingsToolsTests : IDisposable
    {
        private readonly Mock<ILogger<StandingsTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly StandingsTools _standingsTools;

        public StandingsToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<StandingsTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _standingsTools = new StandingsTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCurrentStandings_ValidRequest_ReturnsStandingsData()
        {
            // Arrange
            var expectedJson = TestData.CreateStandingsResponseJson();
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=standings")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _standingsTools.GetCurrentStandings();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetCurrentStandings_ApiReturnsError_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=standings")
                .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await _standingsTools.GetCurrentStandings();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        [Fact]
        public async Task GetCurrentStandings_InvalidJson_ReturnsEmptyList()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/?q=standings")
                .Respond("application/json", "invalid json");

            // Act
            var result = await _standingsTools.GetCurrentStandings();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "JSON parsing error");
        }

        [Fact]
        public async Task GetStandingsByRoundAndYear_ValidParameters_ReturnsStandingsData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var expectedJson = TestData.CreateStandingsResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=standings;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _standingsTools.GetStandingsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(1800, 1)] // Invalid year - too old
        [InlineData(2030, 1)] // Invalid year - too far in future
        [InlineData(2024, 0)] // Invalid round - too low
        [InlineData(2024, 31)] // Invalid round - too high
        public async Task GetStandingsByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Act
            var result = await _standingsTools.GetStandingsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Invalid");
        }

        [Fact]
        public async Task GetStandingsByRoundAndYear_MissingStandingsProperty_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            var jsonWithoutStandingsProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=standings;year={year};round={round}")
                .Respond("application/json", jsonWithoutStandingsProperty);

            // Act
            var result = await _standingsTools.GetStandingsByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'standings' property found");
        }

        [Fact]
        public async Task GetStandingsByRoundAndYear_NetworkError_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=standings;year={year};round={round}")
                .Throw(new HttpRequestException("Network error"));

            // Act
            var result = await _standingsTools.GetStandingsByRoundAndYear(round, year);

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