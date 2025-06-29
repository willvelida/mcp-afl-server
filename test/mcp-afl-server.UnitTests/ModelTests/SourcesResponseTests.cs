using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class SourcesResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void SourcesResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "name": "AFL Official",
            "url": "https://afl.com.au",
            "int": 1,
            "icon": "afl-logo.png"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be("AFL Official");
        response.Url.Should().Be("https://afl.com.au");
        response.Id.Should().Be(1);
        response.Icon.Should().Be("afl-logo.png");
    }

    [Fact]
    public void SourcesResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var sourcesResponse = new SourcesResponse
        {
            Name = "AFL Official",
            Url = "https://afl.com.au",
            Id = 1,
            Icon = "afl-logo.png"
        };

        // Act
        var json = JsonSerializer.Serialize(sourcesResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"name\":\"AFL Official\"");
        json.Should().Contain("\"url\":\"https://afl.com.au\"");
        json.Should().Contain("\"int\":1");
        json.Should().Contain("\"icon\":\"afl-logo.png\"");
    }

    [Fact]
    public void SourcesResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new SourcesResponse
        {
            Name = "The Roar",
            Url = "https://theroar.com.au",
            Id = 2,
            Icon = "roar-icon.svg"
        };

        // Assert
        response.Name.Should().Be("The Roar");
        response.Url.Should().Be("https://theroar.com.au");
        response.Id.Should().Be(2);
        response.Icon.Should().Be("roar-icon.svg");
    }

    [Fact]
    public void SourcesResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new SourcesResponse();

        // Assert
        response.Name.Should().BeNull();
        response.Url.Should().BeNull();
        response.Id.Should().Be(0);
        response.Icon.Should().BeNull();
    }

    [Fact]
    public void SourcesResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "name": null,
            "url": "",
            "icon": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().BeNull();
        response.Url.Should().Be("");
        response.Icon.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void SourcesResponse_IdProperty_ShouldHandleEdgeCases(int id)
    {
        // Arrange
        var json = $$"""
        {
            "int": {{id}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(id);
    }

    [Theory]
    [InlineData("AFL Official")]
    [InlineData("The Roar")]
    [InlineData("")]
    [InlineData("Source with Special Characters !@#")]
    public void SourcesResponse_NameProperty_ShouldHandleVariousValues(string name)
    {
        // Arrange
        var json = $$"""
        {
            "name": "{{name}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("https://afl.com.au")]
    [InlineData("http://example.com")]
    [InlineData("")]
    [InlineData("not-a-url")]
    public void SourcesResponse_UrlProperty_ShouldHandleVariousValues(string url)
    {
        // Arrange
        var json = $$"""
        {
            "url": "{{url}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Url.Should().Be(url);
    }

    [Theory]
    [InlineData("icon.png")]
    [InlineData("logo.svg")]
    [InlineData("")]
    [InlineData("path/to/icon.jpg")]
    public void SourcesResponse_IconProperty_ShouldHandleVariousValues(string icon)
    {
        // Arrange
        var json = $$"""
        {
            "icon": "{{icon}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<SourcesResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Icon.Should().Be(icon);
    }
}