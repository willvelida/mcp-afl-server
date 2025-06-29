using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class GameResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void GameResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "hteam": "Richmond Tigers",
            "ateam": "Carlton Blues",
            "hscore": 105,
            "hgoals": 15,
            "hbehinds": 10,
            "ascore": 92,
            "agoals": 13,
            "abehinds": 14,
            "venue": "MCG",
            "round": 15,
            "roundname": "Round 15",
            "date": "2025-06-28",
            "winner": "Richmond Tigers",
            "is_final": 0,
            "is_grand_final": 0
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeTeam.Should().Be("Richmond Tigers");
        response.AwayTeam.Should().Be("Carlton Blues");
        response.HomeTeamScore.Should().Be(105);
        response.HomeTeamGoals.Should().Be(15);
        response.HomeTeamBehinds.Should().Be(10);
        response.AwayTeamScore.Should().Be(92);
        response.AwayTeamGoals.Should().Be(13);
        response.AwayTeamBehinds.Should().Be(14);
        response.Venue.Should().Be("MCG");
        response.Round.Should().Be(15);
        response.RoundName.Should().Be("Round 15");
        response.Date.Should().Be("2025-06-28");
        response.Winner.Should().Be("Richmond Tigers");
        response.FinalGameType.Should().Be(0);
        response.IsGrandFinal.Should().Be(0);
    }

    [Fact]
    public void GameResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var gameResponse = new GameResponse
        {
            HomeTeam = "Richmond Tigers",
            AwayTeam = "Carlton Blues",
            HomeTeamScore = 105,
            HomeTeamGoals = 15,
            HomeTeamBehinds = 10,
            AwayTeamScore = 92,
            AwayTeamGoals = 13,
            AwayTeamBehinds = 14,
            Venue = "MCG",
            Round = 15,
            RoundName = "Round 15",
            Date = "2025-06-28",
            Winner = "Richmond Tigers",
            FinalGameType = 0,
            IsGrandFinal = 0
        };

        // Act
        var json = JsonSerializer.Serialize(gameResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"hteam\":\"Richmond Tigers\"");
        json.Should().Contain("\"ateam\":\"Carlton Blues\"");
        json.Should().Contain("\"hscore\":105");
        json.Should().Contain("\"hgoals\":15");
        json.Should().Contain("\"hbehinds\":10");
        json.Should().Contain("\"ascore\":92");
        json.Should().Contain("\"agoals\":13");
        json.Should().Contain("\"abehinds\":14");
        json.Should().Contain("\"venue\":\"MCG\"");
        json.Should().Contain("\"round\":15");
        json.Should().Contain("\"roundname\":\"Round 15\"");
        json.Should().Contain("\"date\":\"2025-06-28\"");
        json.Should().Contain("\"winner\":\"Richmond Tigers\"");
        json.Should().Contain("\"is_final\":0");
        json.Should().Contain("\"is_grand_final\":0");
    }

    [Fact]
    public void GameResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new GameResponse
        {
            HomeTeam = "Collingwood",
            AwayTeam = "Essendon",
            HomeTeamScore = 88,
            HomeTeamGoals = 12,
            HomeTeamBehinds = 16,
            AwayTeamScore = 76,
            AwayTeamGoals = 11,
            AwayTeamBehinds = 10,
            Venue = "MCG",
            Round = 10,
            RoundName = "Round 10",
            Date = "2025-07-15",
            Winner = "Collingwood",
            FinalGameType = 0,
            IsGrandFinal = 0
        };

        // Assert
        response.HomeTeam.Should().Be("Collingwood");
        response.AwayTeam.Should().Be("Essendon");
        response.HomeTeamScore.Should().Be(88);
        response.HomeTeamGoals.Should().Be(12);
        response.HomeTeamBehinds.Should().Be(16);
        response.AwayTeamScore.Should().Be(76);
        response.AwayTeamGoals.Should().Be(11);
        response.AwayTeamBehinds.Should().Be(10);
        response.Venue.Should().Be("MCG");
        response.Round.Should().Be(10);
        response.RoundName.Should().Be("Round 10");
        response.Date.Should().Be("2025-07-15");
        response.Winner.Should().Be("Collingwood");
        response.FinalGameType.Should().Be(0);
        response.IsGrandFinal.Should().Be(0);
    }

    [Fact]
    public void GameResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new GameResponse();

        // Assert
        response.HomeTeam.Should().BeNull();
        response.AwayTeam.Should().BeNull();
        response.HomeTeamScore.Should().Be(0);
        response.HomeTeamGoals.Should().Be(0);
        response.HomeTeamBehinds.Should().Be(0);
        response.AwayTeamScore.Should().Be(0);
        response.AwayTeamGoals.Should().Be(0);
        response.AwayTeamBehinds.Should().Be(0);
        response.Venue.Should().BeNull();
        response.Round.Should().Be(0);
        response.RoundName.Should().BeNull();
        response.Date.Should().BeNull();
        response.Winner.Should().BeNull();
        response.FinalGameType.Should().Be(0);
        response.IsGrandFinal.Should().Be(0);
    }

    [Fact]
    public void GameResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "hteam": null,
            "ateam": "",
            "venue": null,
            "roundname": null,
            "date": null,
            "winner": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeTeam.Should().BeNull();
        response.AwayTeam.Should().Be("");
        response.Venue.Should().BeNull();
        response.RoundName.Should().BeNull();
        response.Date.Should().BeNull();
        response.Winner.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void GameResponse_NumericProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "hscore": {{value}},
            "hgoals": {{value}},
            "hbehinds": {{value}},
            "ascore": {{value}},
            "agoals": {{value}},
            "abehinds": {{value}},
            "round": {{value}},
            "is_final": {{value}},
            "is_grand_final": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeTeamScore.Should().Be(value);
        response.HomeTeamGoals.Should().Be(value);
        response.HomeTeamBehinds.Should().Be(value);
        response.AwayTeamScore.Should().Be(value);
        response.AwayTeamGoals.Should().Be(value);
        response.AwayTeamBehinds.Should().Be(value);
        response.Round.Should().Be(value);
        response.FinalGameType.Should().Be(value);
        response.IsGrandFinal.Should().Be(value);
    }

    [Theory]
    [InlineData("2025-06-28")]
    [InlineData("2025-12-31")]
    [InlineData("1990-01-01")]
    [InlineData("")]
    public void GameResponse_DateProperty_ShouldHandleVariousFormats(string date)
    {
        // Arrange
        var json = $$"""
        {
            "date": "{{date}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Date.Should().Be(date);
    }

    [Theory]
    [InlineData("Richmond Tigers", "Carlton Blues")]
    [InlineData("", "")]
    [InlineData("Team with Special Characters !@#", "Another Team")]
    public void GameResponse_TeamNames_ShouldHandleVariousValues(string homeTeam, string awayTeam)
    {
        // Arrange
        var json = $$"""
        {
            "hteam": "{{homeTeam}}",
            "ateam": "{{awayTeam}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeTeam.Should().Be(homeTeam);
        response.AwayTeam.Should().Be(awayTeam);
    }

    [Theory]
    [InlineData("MCG")]
    [InlineData("Marvel Stadium")]
    [InlineData("Adelaide Oval")]
    [InlineData("")]
    public void GameResponse_VenueProperty_ShouldHandleVariousValues(string venue)
    {
        // Arrange
        var json = $$"""
        {
            "venue": "{{venue}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Venue.Should().Be(venue);
    }

    [Theory]
    [InlineData("Richmond Tigers")]
    [InlineData("Carlton Blues")]
    [InlineData("")]
    [InlineData("Draw")]
    public void GameResponse_WinnerProperty_ShouldHandleVariousValues(string winner)
    {
        // Arrange
        var json = $$"""
        {
            "winner": "{{winner}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Winner.Should().Be(winner);
    }

    [Theory]
    [InlineData("Round 1")]
    [InlineData("Round 15")]
    [InlineData("Final")]
    [InlineData("Grand Final")]
    [InlineData("")]
    public void GameResponse_RoundNameProperty_ShouldHandleVariousValues(string roundName)
    {
        // Arrange
        var json = $$"""
        {
            "roundname": "{{roundName}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.RoundName.Should().Be(roundName);
    }

    [Theory]
    [InlineData(0, 1)] // Regular season game, not grand final
    [InlineData(1, 0)] // Final game, not grand final
    [InlineData(1, 1)] // Final game, is grand final
    [InlineData(0, 0)] // Regular season game, not grand final
    public void GameResponse_FinalGameFlags_ShouldHandleVariousCombinations(int finalGameType, int isGrandFinal)
    {
        // Arrange
        var json = $$"""
        {
            "is_final": {{finalGameType}},
            "is_grand_final": {{isGrandFinal}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<GameResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.FinalGameType.Should().Be(finalGameType);
        response.IsGrandFinal.Should().Be(isGrandFinal);
    }
}