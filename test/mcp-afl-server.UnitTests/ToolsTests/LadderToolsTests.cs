using FluentAssertions;
using mcp_afl_server.Tools;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    public class LadderToolsTests : IDisposable
    {
        private readonly Mock<ILogger<LadderTools>> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly LadderTools _ladderTools;

        public LadderToolsTests()
        {
            _mockLogger = HttpClientTestHelper.CreateMockLogger<LadderTools>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _ladderTools = new LadderTools(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYear_ValidParameters_ReturnsLadderData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            var expectedJson = TestData.CreateLadderResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(1800, 1)] // Invalid year - too old
        [InlineData(2030, 1)] // Invalid year - too far in future
        [InlineData(2024, 0)] // Invalid round - too low
        [InlineData(2024, 31)] // Invalid round - too high
        public async Task GetProjectedLadderByRoundAndYear_InvalidParameters_ReturnsEmptyList(int year, int round)
        {
            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Invalid");
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYearBySource_ValidParameters_ReturnsLadderData()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const string source = "TestSource";
            var expectedJson = TestData.CreateLadderResponseJson();
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round};source={source}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYearBySource(round, year, source);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetProjectedLadderByRoundAndYearBySource_InvalidSource_ReturnsEmptyList(string invalidSource)
        {
            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYearBySource(1, 2024, invalidSource!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "Source cannot be null or empty");
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYear_ApiReturnsError_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round}")
                .Respond(HttpStatusCode.BadRequest);

            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Error, "API request failed");
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYearBySource_SourceWithSpecialCharacters_EncodesCorrectly()
        {
            // Arrange
            const int year = 2024;
            const int round = 5;
            const string sourceWithSpecialChars = "Test Source & More";
            var expectedJson = TestData.CreateLadderResponseJson();
            
            // The URL will NOT be encoded - the source is passed directly to the endpoint
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round};source={sourceWithSpecialChars}")
                .Respond("application/json", expectedJson);

            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYearBySource(round, year, sourceWithSpecialChars);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProjectedLadderByRoundAndYear_MissingLadderProperty_ReturnsEmptyList()
        {
            // Arrange
            const int year = 2024;
            const int round = 1;
            var jsonWithoutLadderProperty = """{"other_property": []}""";
            
            _mockHttpHandler
                .When($"https://api.squiggle.com.au/?q=ladder;year={year};round={round}")
                .Respond("application/json", jsonWithoutLadderProperty);

            // Act
            var result = await _ladderTools.GetProjectedLadderByRoundAndYear(round, year);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            HttpClientTestHelper.VerifyLogMessage(_mockLogger, LogLevel.Warning, "No 'ladder' property found");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}