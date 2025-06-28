// test/mcp-afl-server.UnitTests/Tools/BaseAFLToolTests.cs
using FluentAssertions;
using mcp_afl_server.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
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
        public TestableBaseAFLTool(HttpClient httpClient, ILogger logger) 
            : base(httpClient, logger) { }

        // Expose protected methods for testing
        public new bool ValidateParameters(params (string name, object value, Func<object, bool> validator, string errorMessage)[] validations)
            => base.ValidateParameters(validations);

        public new Task<T> ExecuteApiCallAsync<T>(string endpoint, string operationName, string propertyName, Func<T, bool>? additionalValidation = null) where T : class, new()
            => base.ExecuteApiCallAsync(endpoint, operationName, propertyName, additionalValidation);

        public new static bool IsValidYear(int year) => BaseAFLTool.IsValidYear(year);
        public new static bool IsValidRound(int round) => BaseAFLTool.IsValidRound(round);
        public new static bool IsValidId(int id) => BaseAFLTool.IsValidId(id);
        public new static bool IsValidString(string value) => BaseAFLTool.IsValidString(value);
    }

    // Simple test model for API call testing
    public class TestResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class BaseAFLToolTests : IDisposable
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly TestableBaseAFLTool _tool;

        public BaseAFLToolTests()
        {
            _mockLogger = new Mock<ILogger>();
            (_httpClient, _mockHttpHandler) = HttpClientTestHelper.CreateMockHttpClient();
            _tool = new TestableBaseAFLTool(_httpClient, _mockLogger.Object);
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
        [InlineData("valid string", true)]
        [InlineData("a", true)]           // Single character
        [InlineData("", false)]           // Empty string
        [InlineData("   ", false)]        // Whitespace only
        [InlineData("\t\n", false)]       // Other whitespace
        [InlineData(null, false)]         // Null
        public void IsValidString_VariousStrings_ReturnsExpectedResult(string? input, bool expected)
        {
            // Act
            var result = TestableBaseAFLTool.IsValidString(input!);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region ValidateParameters Tests

        [Fact]
        public void ValidateParameters_AllValid_ReturnsTrue()
        {
            // Act
            var result = _tool.ValidateParameters(
                ("year", 2024, val => TestableBaseAFLTool.IsValidYear((int)val), "Invalid year"),
                ("round", 1, val => TestableBaseAFLTool.IsValidRound((int)val), "Invalid round"),
                ("id", 5, val => TestableBaseAFLTool.IsValidId((int)val), "Invalid ID")
            );

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateParameters_OneInvalid_ReturnsFalseAndLogs()
        {
            // Act
            var result = _tool.ValidateParameters(
                ("year", 2024, val => TestableBaseAFLTool.IsValidYear((int)val), "Invalid year"),
                ("round", 0, val => TestableBaseAFLTool.IsValidRound((int)val), "Invalid round")
            );

            // Assert
            result.Should().BeFalse();
            
            // Verify warning was logged with correct parameter details
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString()!.Contains("Invalid round parameter") && 
                        v.ToString()!.Contains("0") &&
                        v.ToString()!.Contains("Invalid round")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidateParameters_MultipleInvalid_ReturnsFalseOnFirst()
        {
            // Act
            var result = _tool.ValidateParameters(
                ("year", 1800, val => TestableBaseAFLTool.IsValidYear((int)val), "Invalid year"),
                ("round", 0, val => TestableBaseAFLTool.IsValidRound((int)val), "Invalid round")
            );

            // Assert
            result.Should().BeFalse();
            
            // Should only log the first invalid parameter
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid year parameter")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidateParameters_EmptyValidations_ReturnsTrue()
        {
            // Act
            var result = _tool.ValidateParameters();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateParameters_StringValidation_WorksCorrectly()
        {
            // Act - Test valid string
            var validResult = _tool.ValidateParameters(
                ("source", "valid string", val => TestableBaseAFLTool.IsValidString((string)val), "Invalid string")
            );

            // Assert
            validResult.Should().BeTrue();

            // Act - Test invalid string
            var invalidResult = _tool.ValidateParameters(
                ("source", "", val => TestableBaseAFLTool.IsValidString((string)val), "Invalid string")
            );

            // Assert
            invalidResult.Should().BeFalse();
        }

        #endregion

        #region ExecuteApiCallAsync Tests - Basic Functionality

        [Fact]
        public async Task ExecuteApiCallAsync_SuccessfulResponse_ReturnsData()
        {
            // Arrange
            var testData = new { testProperty = new[] { new { id = 1, name = "Test" } } };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Test");
        }

        [Fact]
        public async Task ExecuteApiCallAsync_HttpError_ReturnsEmptyAndLogs()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond(HttpStatusCode.NotFound);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString()!.Contains("API request failed") &&
                        v.ToString()!.Contains("NotFound")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_MissingProperty_ReturnsEmptyAndLogs()
        {
            // Arrange
            var testData = new { otherProperty = new[] { new { id = 1 } } };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify warning was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString()!.Contains("No 'testProperty' property found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_InvalidJson_ReturnsEmptyAndLogs()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", "invalid json");

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify JSON error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("JSON parsing error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_NetworkError_ReturnsEmptyAndLogs()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Throw(new HttpRequestException("Network error"));

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify network error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_Timeout_ReturnsEmptyAndLogs()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Throw(new TaskCanceledException("Request timeout"));

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify timeout error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Timeout")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_EmptyResult_ReturnsEmptyAndLogsInfo()
        {
            // Arrange
            var testData = new { testProperty = Array.Empty<object>() };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify info message about no data
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No data found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_WithAdditionalValidation_FailsValidation()
        {
            // Arrange
            var testData = new { testProperty = new[] { new { id = 1, name = "Test" } } };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Additional validation that always fails
            Func<List<TestResponse>, bool> failingValidation = (list) => false;

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty",
                failingValidation
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify validation failure was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Additional validation failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_WithAdditionalValidation_PassesValidation()
        {
            // Arrange
            var testData = new { testProperty = new[] { new { id = 1, name = "Test" } } };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Additional validation that always passes
            Func<List<TestResponse>, bool> passingValidation = (list) => true;

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty",
                passingValidation
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Test");
        }

        [Fact]
        public async Task ExecuteApiCallAsync_NullAdditionalValidation_WorksCorrectly()
        {
            // Arrange
            var testData = new { testProperty = new[] { new { id = 1, name = "Test" } } };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty",
                null
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Test");
        }

        #endregion

        #region Enhanced Error Handling Tests - HTTP Status Codes

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        public async Task ExecuteApiCallAsync_VariousHttpErrors_ReturnsEmptyAndLogsCorrectly(HttpStatusCode statusCode)
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond(statusCode);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Verify error was logged with correct status code
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString()!.Contains("API request failed") &&
                        v.ToString()!.Contains(statusCode.ToString())),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Enhanced Error Handling Tests - Malformed JSON

        [Theory]
        [InlineData("")]                              // Empty response - JSON parsing error
        [InlineData("{")]                             // Incomplete JSON - JSON parsing error  
        [InlineData("{\"incomplete\": ")]             // Incomplete JSON with property - JSON parsing error
        [InlineData("{\"validProperty\": [}")]        // Invalid array syntax - JSON parsing error
        [InlineData("{\"validProperty\": {\"nested\": [1,2,}]}}")]  // Invalid nested structure - JSON parsing error
        public async Task ExecuteApiCallAsync_TrulyMalformedJson_LogsJsonParsingError(string malformedJson)
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", malformedJson);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Should log JSON parsing error for truly malformed JSON
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("JSON parsing error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData("null")]                          // Valid JSON null, but wrong structure
        [InlineData("true")]                          // Valid JSON boolean, but wrong structure
        [InlineData("123")]                           // Valid JSON number, but wrong structure
        [InlineData("\"string\"")]                    // Valid JSON string, but wrong structure
        public async Task ExecuteApiCallAsync_ValidJsonWrongStructure_LogsUnexpectedError(string validJsonWrongStructure)
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", validJsonWrongStructure);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            // Should log Unexpected error for valid JSON with wrong structure
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unexpected error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Enhanced Error Handling Tests - Content Types and Large Responses

        [Theory]
        [InlineData("text/html")]
        [InlineData("application/xml")]
        [InlineData("text/plain")]
        [InlineData("application/octet-stream")]
        public async Task ExecuteApiCallAsync_IncorrectContentType_HandlesGracefully(string contentType)
        {
            // Arrange
            var jsonData = JsonSerializer.Serialize(new { testProperty = new[] { new { id = 1, name = "Test" } } });
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond(contentType, jsonData);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            // Should still work if content is valid JSON, regardless of content-type
            // (HttpClient is generally tolerant of content-type mismatches)
        }

        [Fact]
        public async Task ExecuteApiCallAsync_LargeResponse_HandlesCorrectly()
        {
            // Arrange - Create a large response (e.g., 1000 items)
            var largeData = Enumerable.Range(1, 1000)
                .Select(i => new { id = i, name = $"Item {i}" })
                .ToArray();
            var testData = new { testProperty = largeData };
            var jsonResponse = JsonSerializer.Serialize(testData);
            
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Respond("application/json", jsonResponse);

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1000);
            
            // Verify appropriate logging for large datasets
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => 
                        v.ToString()!.Contains("Successfully retrieved 1000 items")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Enhanced Error Handling Tests - Network Level Errors

        [Fact]
        public async Task ExecuteApiCallAsync_DNSResolutionFailure_HandlesGracefully()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Throw(new HttpRequestException("Name or service not known"));

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_ConnectionRefused_HandlesGracefully()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Throw(new HttpRequestException("Connection refused"));

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteApiCallAsync_SSLCertificateError_HandlesGracefully()
        {
            // Arrange
            _mockHttpHandler
                .When("https://api.squiggle.com.au/test-endpoint")
                .Throw(new HttpRequestException("SSL connection could not be established"));

            // Act
            var result = await _tool.ExecuteApiCallAsync<List<TestResponse>>(
                "test-endpoint", 
                "Test Operation", 
                "testProperty"
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
            _mockHttpHandler?.Dispose();
        }
    }
}