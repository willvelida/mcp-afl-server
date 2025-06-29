using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class TeamResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void TeamResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "name": "Richmond Tigers",
            "abbrev": "RICH",
            "logo": "richmond-logo.png",
            "id": 1,
            "debut": 1908,
            "retirement": 0
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be("Richmond Tigers");
        response.Abbrevation.Should().Be("RICH");
        response.Logo.Should().Be("richmond-logo.png");
        response.Id.Should().Be(1);
        response.Debut.Should().Be(1908);
        response.Retirement.Should().Be(0);
    }

    [Fact]
    public void TeamResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var teamResponse = new TeamResponse
        {
            Name = "Richmond Tigers",
            Abbrevation = "RICH",
            Logo = "richmond-logo.png",
            Id = 1,
            Debut = 1908,
            Retirement = 0
        };

        // Act
        var json = JsonSerializer.Serialize(teamResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"name\":\"Richmond Tigers\"");
        json.Should().Contain("\"abbrev\":\"RICH\"");
        json.Should().Contain("\"logo\":\"richmond-logo.png\"");
        json.Should().Contain("\"id\":1");
        json.Should().Contain("\"debut\":1908");
        json.Should().Contain("\"retirement\":0");
    }

    [Fact]
    public void TeamResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new TeamResponse
        {
            Name = "Collingwood Magpies",
            Abbrevation = "COLL",
            Logo = "collingwood-logo.svg",
            Id = 2,
            Debut = 1892,
            Retirement = 0
        };

        // Assert
        response.Name.Should().Be("Collingwood Magpies");
        response.Abbrevation.Should().Be("COLL");
        response.Logo.Should().Be("collingwood-logo.svg");
        response.Id.Should().Be(2);
        response.Debut.Should().Be(1892);
        response.Retirement.Should().Be(0);
    }

    [Fact]
    public void TeamResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new TeamResponse();

        // Assert
        response.Name.Should().BeNull();
        response.Abbrevation.Should().BeNull();
        response.Logo.Should().BeNull();
        response.Id.Should().Be(0);
        response.Debut.Should().Be(0);
        response.Retirement.Should().Be(0);
    }

    [Fact]
    public void TeamResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "name": null,
            "abbrev": "",
            "logo": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().BeNull();
        response.Abbrevation.Should().Be("");
        response.Logo.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void TeamResponse_NumericProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "id": {{value}},
            "debut": {{value}},
            "retirement": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(value);
        response.Debut.Should().Be(value);
        response.Retirement.Should().Be(value);
    }

    [Theory]
    [InlineData("Richmond Tigers")]
    [InlineData("Collingwood Magpies")]
    [InlineData("")]
    [InlineData("Team with Special Characters !@#")]
    public void TeamResponse_NameProperty_ShouldHandleVariousValues(string name)
    {
        // Arrange
        var json = $$"""
        {
            "name": "{{name}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("RICH")]
    [InlineData("COLL")]
    [InlineData("")]
    [InlineData("ABBR")]
    public void TeamResponse_AbbrevationProperty_ShouldHandleVariousValues(string abbrev)
    {
        // Arrange
        var json = $$"""
        {
            "abbrev": "{{abbrev}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Abbrevation.Should().Be(abbrev);
    }

    [Theory]
    [InlineData("logo.png")]
    [InlineData("team-logo.svg")]
    [InlineData("")]
    [InlineData("path/to/logo.jpg")]
    public void TeamResponse_LogoProperty_ShouldHandleVariousValues(string logo)
    {
        // Arrange
        var json = $$"""
        {
            "logo": "{{logo}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Logo.Should().Be(logo);
    }

    [Theory]
    [InlineData(1897, 0)] // Active team
    [InlineData(1900, 1920)] // Retired team
    [InlineData(2000, 0)] // Modern team
    public void TeamResponse_DebutAndRetirementCombinations_ShouldBeHandledCorrectly(int debut, int retirement)
    {
        // Arrange
        var json = $$"""
        {
            "debut": {{debut}},
            "retirement": {{retirement}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TeamResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Debut.Should().Be(debut);
        response.Retirement.Should().Be(retirement);
    }
}