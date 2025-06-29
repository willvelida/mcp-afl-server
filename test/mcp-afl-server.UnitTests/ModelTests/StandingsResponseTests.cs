using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class StandingsResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void StandingsResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "rank": 1,
            "behinds_for": 245,
            "goals_for": 298,
            "behinds_against": 198,
            "goals_against": 225,
            "pts": 72,
            "wins": 18,
            "draws": 0,
            "losses": 4,
            "played": 22,
            "percentage": 125.5,
            "for": 2033,
            "against": 1548,
            "name": "Melbourne"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StandingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Rank.Should().Be(1);
        response.BehindsFor.Should().Be(245);
        response.GoalsFor.Should().Be(298);
        response.BehindsAgainst.Should().Be(198);
        response.GoalsAgainst.Should().Be(225);
        response.Points.Should().Be(72);
        response.Wins.Should().Be(18);
        response.Draws.Should().Be(0);
        response.Losses.Should().Be(4);
        response.Played.Should().Be(22);
        response.Percentage.Should().Be(125.5);
        response.For.Should().Be(2033);
        response.Against.Should().Be(1548);
        response.TeamName.Should().Be("Melbourne");
    }

    [Fact]
    public void StandingsResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var standingsResponse = new StandingsResponse
        {
            Rank = 1,
            BehindsFor = 245,
            GoalsFor = 298,
            BehindsAgainst = 198,
            GoalsAgainst = 225,
            Points = 72,
            Wins = 18,
            Draws = 0,
            Losses = 4,
            Played = 22,
            Percentage = 125.5,
            For = 2033,
            Against = 1548,
            TeamName = "Melbourne"
        };

        // Act
        var json = JsonSerializer.Serialize(standingsResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"rank\":1");
        json.Should().Contain("\"behinds_for\":245");
        json.Should().Contain("\"goals_for\":298");
        json.Should().Contain("\"behinds_against\":198");
        json.Should().Contain("\"goals_against\":225");
        json.Should().Contain("\"pts\":72");
        json.Should().Contain("\"wins\":18");
        json.Should().Contain("\"draws\":0");
        json.Should().Contain("\"losses\":4");
        json.Should().Contain("\"played\":22");
        json.Should().Contain("\"percentage\":125.5");
        json.Should().Contain("\"for\":2033");
        json.Should().Contain("\"against\":1548");
        json.Should().Contain("\"name\":\"Melbourne\"");
    }

    [Fact]
    public void StandingsResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new StandingsResponse
        {
            Rank = 2,
            BehindsFor = 220,
            GoalsFor = 275,
            BehindsAgainst = 210,
            GoalsAgainst = 240,
            Points = 68,
            Wins = 17,
            Draws = 0,
            Losses = 5,
            Played = 22,
            Percentage = 118.2,
            For = 1870,
            Against = 1650,
            TeamName = "Brisbane Lions"
        };

        // Assert
        response.Rank.Should().Be(2);
        response.BehindsFor.Should().Be(220);
        response.GoalsFor.Should().Be(275);
        response.BehindsAgainst.Should().Be(210);
        response.GoalsAgainst.Should().Be(240);
        response.Points.Should().Be(68);
        response.Wins.Should().Be(17);
        response.Draws.Should().Be(0);
        response.Losses.Should().Be(5);
        response.Played.Should().Be(22);
        response.Percentage.Should().Be(118.2);
        response.For.Should().Be(1870);
        response.Against.Should().Be(1650);
        response.TeamName.Should().Be("Brisbane Lions");
    }

    [Fact]
    public void StandingsResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new StandingsResponse();

        // Assert
        response.Rank.Should().Be(0);
        response.BehindsFor.Should().Be(0);
        response.GoalsFor.Should().Be(0);
        response.BehindsAgainst.Should().Be(0);
        response.GoalsAgainst.Should().Be(0);
        response.Points.Should().Be(0);
        response.Wins.Should().Be(0);
        response.Draws.Should().Be(0);
        response.Losses.Should().Be(0);
        response.Played.Should().Be(0);
        response.Percentage.Should().Be(0);
        response.For.Should().Be(0);
        response.Against.Should().Be(0);
        response.TeamName.Should().BeNull();
    }

    [Fact]
    public void StandingsResponse_NullTeamName_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "name": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StandingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.TeamName.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void StandingsResponse_IntegerProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "rank": {{value}},
            "behinds_for": {{value}},
            "goals_for": {{value}},
            "behinds_against": {{value}},
            "goals_against": {{value}},
            "pts": {{value}},
            "wins": {{value}},
            "draws": {{value}},
            "losses": {{value}},
            "played": {{value}},
            "for": {{value}},
            "against": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StandingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Rank.Should().Be(value);
        response.BehindsFor.Should().Be(value);
        response.GoalsFor.Should().Be(value);
        response.BehindsAgainst.Should().Be(value);
        response.GoalsAgainst.Should().Be(value);
        response.Points.Should().Be(value);
        response.Wins.Should().Be(value);
        response.Draws.Should().Be(value);
        response.Losses.Should().Be(value);
        response.Played.Should().Be(value);
        response.For.Should().Be(value);
        response.Against.Should().Be(value);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(100.0)]
    [InlineData(125.5)]
    [InlineData(999.99)]
    [InlineData(double.MaxValue)]
    public void StandingsResponse_PercentageProperty_ShouldHandleEdgeCases(double percentage)
    {
        // Arrange
        var json = $$"""
        {
            "percentage": {{percentage}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StandingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Percentage.Should().Be(percentage);
    }

    [Theory]
    [InlineData("Melbourne")]
    [InlineData("Brisbane Lions")]
    [InlineData("")]
    [InlineData("Team with Special Characters !@#")]
    public void StandingsResponse_TeamNameProperty_ShouldHandleVariousValues(string teamName)
    {
        // Arrange
        var json = $$"""
        {
            "name": "{{teamName}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<StandingsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.TeamName.Should().Be(teamName);
    }
}