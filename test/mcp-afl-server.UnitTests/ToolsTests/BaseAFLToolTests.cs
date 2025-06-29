// test/mcp-afl-server.UnitTests/Tools/BaseAFLToolTests.cs
using FluentAssertions;
using mcp_afl_server.Services;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mcp_afl_server.UnitTests.ToolsTests
{
    // Test implementation of BaseAFLTool to test protected methods
    public class TestableBaseAFLTool : BaseAFLTool
    {
        public TestableBaseAFLTool(
            HttpClient httpClient, 
            ILogger logger,
            IAuthenticationService authenticationService) 
            : base(httpClient, logger, authenticationService) { }

        // Expose protected methods for testing
        public new bool ValidateParameters(params (string name, object value, Func<object, bool> validator, string errorMessage)[] validations)
            => base.ValidateParameters(validations);

        public new Task<T> ExecuteApiCallAsync<T>(string endpoint, string operationName, string propertyName, Func<T, bool>? additionalValidation = null) where T : class, new()
            => base.ExecuteApiCallAsync(endpoint, operationName, propertyName, additionalValidation);

        public new static bool IsValidYear(int year) => BaseAFLTool.IsValidYear(year);
        public new static bool IsValidRound(int round) => BaseAFLTool.IsValidRound(round);
        public new static bool IsValidId(int id) => BaseAFLTool.IsValidId(id);
        public new static bool IsValidString(string value) => BaseAFLTool.IsValidString(value);

        // Expose authentication methods for testing
        public new Task<User> GetCurrentUserAsync() => base.GetCurrentUserAsync();
        public new bool IsAuthenticated() => base.IsAuthenticated();
    }

    // Simple test model for API call testing
    public class TestResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class BaseAFLToolTests : AuthenticatedToolTestBase
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly TestableBaseAFLTool _tool;

        public BaseAFLToolTests() : base()
        {
            _mockLogger = new Mock<ILogger>();
            _tool = new TestableBaseAFLTool(HttpClient, _mockLogger.Object, MockAuthenticationService.Object);
        }

        #region Validation Tests

        [Theory]
        [InlineData(1897, true)]  // First AFL year
        [InlineData(2024, true)]  // Current year
        [InlineData(2026, true)]  // Next year + 1
        [InlineData(1896, false)] // Before AFL
        [InlineData(2027, false)] // Too far in future
        public void IsValidYear_VariousYears_ReturnsExpectedResult(int year, bool expected)
        {
            // Act
            var result = TestableBaseAFLTool.IsValidYear(year);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, true)]   // Valid round
        [InlineData(23, true)]  // High valid round
        [InlineData(30, true)]  // Max valid round
        [InlineData(0, false)]  // Too low
        [InlineData(31, false)] // Too high
        [InlineData(-1, false)] // Negative
        public void IsValidRound_VariousRounds_ReturnsExpectedResult(int round, bool expected)
        {
            // Act
            var result = TestableBaseAFLTool.IsValidRound(round);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, true)]     // Valid ID
        [InlineData(999999, true)] // Large valid ID
        [InlineData(0, false)]    // Zero is invalid
        [InlineData(-1, false)]   // Negative is invalid
        public void IsValidId_VariousIds_ReturnsExpectedResult(int id, bool expected)
        {
            // Act
            var result = TestableBaseAFLTool.IsValidId(id);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("valid", true)]     // Valid string
        [InlineData("", false)]         // Empty string
        [InlineData("   ", false)]      // Whitespace
        public void IsValidString_VariousStrings_ReturnsExpectedResult(string value, bool expected)
        {
            // Act
            var result = TestableBaseAFLTool.IsValidString(value);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidString_Null_ReturnsFalse()
        {
            // Act
            var result = TestableBaseAFLTool.IsValidString(null!);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Parameter Validation Tests

        [Fact]
        public void ValidateParameters_AllValid_ReturnsTrue()
        {
            // Act
            var result = _tool.ValidateParameters(
                ("year", 2024, val => TestableBaseAFLTool.IsValidYear((int)val), "Invalid year"),
                ("round", 5, val => TestableBaseAFLTool.IsValidRound((int)val), "Invalid round"));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateParameters_OneInvalid_ReturnsFalse()
        {
            // Act
            var result = _tool.ValidateParameters(
                ("year", 2024, val => TestableBaseAFLTool.IsValidYear((int)val), "Invalid year"),
                ("round", 999, val => TestableBaseAFLTool.IsValidRound((int)val), "Invalid round"));

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetCurrentUserAsync_Authenticated_ReturnsUser()
        {
            // Arrange
            var expectedUser = CreateTestUser("test-user-123");
            SetupAuthenticatedUser("test-user-123", "test@example.com");

            // Act
            var result = await _tool.GetCurrentUserAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("test-user-123");
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUserAsync_Unauthenticated_ThrowsException()
        {
            // Arrange
            SetupUnauthenticatedUser();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _tool.GetCurrentUserAsync());
            MockAuthenticationService.Verify(x => x.GetCurrentUserAsync(), Times.Once);
        }

        [Fact]
        public void IsAuthenticated_WhenAuthenticated_ReturnsTrue()
        {
            // Arrange
            SetupAuthenticatedUser();

            // Act
            var result = _tool.IsAuthenticated();

            // Assert
            result.Should().BeTrue();
            MockAuthenticationService.Verify(x => x.IsAuthenticated(), Times.Once);
        }

        [Fact]
        public void IsAuthenticated_WhenNotAuthenticated_ReturnsFalse()
        {
            // Arrange
            SetupUnauthenticatedUser();

            // Act
            var result = _tool.IsAuthenticated();

            // Assert
            result.Should().BeFalse();
            MockAuthenticationService.Verify(x => x.IsAuthenticated(), Times.Once);
        }

        #endregion

        #region API Call Tests

        [Fact]
        public async Task ExecuteApiCallAsync_SuccessfulResponse_ReturnsDeserializedData()
        {
            // Arrange
            var testData = new List<TestResponse> 
            { 
                new TestResponse { Id = 1, Name = "Test" } 
            };
            var responseJson = JsonSerializer.Serialize(new { games = testData });

            MockHttpHandler
                .When("https://api.squiggle.com.au/test")
                .Respond("application/json", responseJson);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>("test", "Test Operation", "games");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Test");
        }

        [Fact]
        public async Task ExecuteApiCallAsync_HttpError_ReturnsEmptyResult()
        {
            // Arrange
            MockHttpHandler
                .When("https://api.squiggle.com.au/test")
                .Respond(HttpStatusCode.InternalServerError);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>("test", "Test Operation", "games");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteApiCallAsync_MissingProperty_ReturnsEmptyResult()
        {
            // Arrange
            var responseJson = JsonSerializer.Serialize(new { wrongProperty = new List<TestResponse>() });

            MockHttpHandler
                .When("https://api.squiggle.com.au/test")
                .Respond("application/json", responseJson);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>("test", "Test Operation", "games");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}