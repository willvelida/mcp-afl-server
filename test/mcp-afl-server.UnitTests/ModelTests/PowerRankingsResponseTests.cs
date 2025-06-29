using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class PowerRankingsResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void PowerRankingsResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "sourceid": 1,
            "power": "1650.5",
            "team": "Melbourne",
            "source": "AFL Official",
            "rank": 1,
            "teamid": 5,
            "dummy": 0,
            "updated": "2025-06-28T10:30:00Z",
            "year": 2025,
            "round": 15
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PowerRankingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.SourceId.Should().Be(1);
        response.Power.Should().Be("1650.5");
        response.Team.Should().Be("Melbourne");
        response.Source.Should().Be("AFL Official");
        response.Rank.Should().Be(1);
        response.TeamId.Should().Be(5);
        response.Dummy.Should().Be(0);
        response.Updated.Should().Be("2025-06-28T10:30:00Z");
        response.Year.Should().Be(2025);
        response.Round.Should().Be(15);
    }

    [Fact]
    public void PowerRankingsResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var powerRankingsResponse = new PowerRankingsResponse
        {
            SourceId = 1,
            Power = "1650.5",
            Team = "Melbourne",
            Source = "AFL Official",
            Rank = 1,
            TeamId = 5,
            Dummy = 0,
            Updated = "2025-06-28T10:30:00Z",
            Year = 2025,
            Round = 15
        };

        // Act
        var json = JsonSerializer.Serialize(powerRankingsResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"sourceid\":1");
        json.Should().Contain("\"power\":\"1650.5\"");
        json.Should().Contain("\"team\":\"Melbourne\"");
        json.Should().Contain("\"source\":\"AFL Official\"");
        json.Should().Contain("\"rank\":1");
        json.Should().Contain("\"teamid\":5");
        json.Should().Contain("\"dummy\":0");
        json.Should().Contain("\"updated\":\"2025-06-28T10:30:00Z\"");
        json.Should().Contain("\"year\":2025");
        json.Should().Contain("\"round\":15");
    }

    [Fact]
    public void PowerRankingsResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new PowerRankingsResponse
        {
            SourceId = 2,
            Power = "1525.8",
            Team = "Brisbane Lions",
            Source = "Expert Analysis",
            Rank = 2,
            TeamId = 3,
            Dummy = 1,
            Updated = "2025-07-15T14:20:00Z",
            Year = 2025,
            Round = 12
        };

        // Assert
        response.SourceId.Should().Be(2);
        response.Power.Should().Be("1525.8");
        response.Team.Should().Be("Brisbane Lions");
        response.Source.Should().Be("Expert Analysis");
        response.Rank.Should().Be(2);
        response.TeamId.Should().Be(3);
        response.Dummy.Should().Be(1);
        response.Updated.Should().Be("2025-07-15T14:20:00Z");
        response.Year.Should().Be(2025);
        response.Round.Should().Be(12);
    }

    [Fact]
    public void PowerRankingsResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new PowerRankingsResponse();

        // Assert
        response.SourceId.Should().Be(0);
        response.Power.Should().BeNull();
        response.Team.Should().BeNull();
        response.Source.Should().BeNull();
        response.Rank.Should().Be(0);
        response.TeamId.Should().Be(0);
        response.Dummy.Should().Be(0);
        response.Updated.Should().BeNull();
        response.Year.Should().Be(0);
        response.Round.Should().Be(0);
    }

    [Fact]
    public void PowerRankingsResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "power": null,
            "team": "",
            "source": null,
            "updated": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PowerRankingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Power.Should().BeNull();
        response.Team.Should().Be("");
        response.Source.Should().BeNull();
        response.Updated.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void PowerRankingsResponse_NumericProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "sourceid": {{value}},
            "rank": {{value}},
            "teamid": {{value}},
            "dummy": {{value}},
            "year": {{value}},
            "round": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PowerRankingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.SourceId.Should().Be(value);
        response.Rank.Should().Be(value);
        response.TeamId.Should().Be(value);
        response.Dummy.Should().Be(value);
        response.Year.Should().Be(value);
        response.Round.Should().Be(value);
    }

    [Theory]
    [InlineData("1650.5")]
    [InlineData("0.0")]
    [InlineData("2000.75")]
    [InlineData("")]
    public void PowerRankingsResponse_PowerProperty_ShouldHandleVariousValues(string power)
    {
        // Arrange
        var json = $$"""
        {
            "power": "{{power}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PowerRankingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Power.Should().Be(power);
    }

    [Theory]
    [InlineData("Melbourne")]
    [InlineData("Brisbane Lions")]
    [InlineData("")]
    [InlineData("Team with Special Characters !@#")]
    public void PowerRankingsResponse_TeamProperty_ShouldHandleVariousValues(string team)
    {
        // Arrange
        var json = $$"""
        {
            "team": "{{team}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<PowerRankingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Team.Should().Be(team);
    }
}