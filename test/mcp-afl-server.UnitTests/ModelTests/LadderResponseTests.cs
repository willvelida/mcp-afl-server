using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class LadderResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void LadderResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "team": "Melbourne",
            "swarms": ["swarm1", "swarm2", "swarm3"],
            "wins": "18",
            "teamid": 1,
            "updated": "2025-06-28T10:30:00Z",
            "dummy": 0,
            "year": 2025,
            "round": 15,
            "sourceid": 1,
            "mean_rank": "1.5",
            "source": "AFL Official",
            "rank": 1
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Team.Should().Be("Melbourne");
        response.Swarms.Should().NotBeNull();
        response.Swarms.Should().HaveCount(3);
        response.Swarms.Should().Equal("swarm1", "swarm2", "swarm3");
        response.Wins.Should().Be("18");
        response.TeamId.Should().Be(1);
        response.Updated.Should().Be("2025-06-28T10:30:00Z");
        response.Dummy.Should().Be(0);
        response.Year.Should().Be(2025);
        response.Round.Should().Be(15);
        response.SourceId.Should().Be(1);
        response.MeanRank.Should().Be("1.5");
        response.Source.Should().Be("AFL Official");
        response.Rank.Should().Be(1);
    }

    [Fact]
    public void LadderResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var ladderResponse = new LadderResponse
        {
            Team = "Melbourne",
            Swarms = new List<string> { "swarm1", "swarm2", "swarm3" },
            Wins = "18",
            TeamId = 1,
            Updated = "2025-06-28T10:30:00Z",
            Dummy = 0,
            Year = 2025,
            Round = 15,
            SourceId = 1,
            MeanRank = "1.5",
            Source = "AFL Official",
            Rank = 1
        };

        // Act
        var json = JsonSerializer.Serialize(ladderResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"team\":\"Melbourne\"");
        json.Should().Contain("\"swarms\":[\"swarm1\",\"swarm2\",\"swarm3\"]");
        json.Should().Contain("\"wins\":\"18\"");
        json.Should().Contain("\"teamid\":1");
        json.Should().Contain("\"updated\":\"2025-06-28T10:30:00Z\"");
        json.Should().Contain("\"dummy\":0");
        json.Should().Contain("\"year\":2025");
        json.Should().Contain("\"round\":15");
        json.Should().Contain("\"sourceid\":1");
        json.Should().Contain("\"mean_rank\":\"1.5\"");
        json.Should().Contain("\"source\":\"AFL Official\"");
        json.Should().Contain("\"rank\":1");
    }

    [Fact]
    public void LadderResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new LadderResponse
        {
            Team = "Brisbane Lions",
            Swarms = new List<string> { "lions", "gabba" },
            Wins = "16",
            TeamId = 2,
            Updated = "2025-07-15T14:20:00Z",
            Dummy = 1,
            Year = 2025,
            Round = 12,
            SourceId = 2,
            MeanRank = "2.3",
            Source = "Expert Analysis",
            Rank = 2
        };

        // Assert
        response.Team.Should().Be("Brisbane Lions");
        response.Swarms.Should().Equal("lions", "gabba");
        response.Wins.Should().Be("16");
        response.TeamId.Should().Be(2);
        response.Updated.Should().Be("2025-07-15T14:20:00Z");
        response.Dummy.Should().Be(1);
        response.Year.Should().Be(2025);
        response.Round.Should().Be(12);
        response.SourceId.Should().Be(2);
        response.MeanRank.Should().Be("2.3");
        response.Source.Should().Be("Expert Analysis");
        response.Rank.Should().Be(2);
    }

    [Fact]
    public void LadderResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new LadderResponse();

        // Assert
        response.Team.Should().BeNull();
        response.Swarms.Should().BeNull();
        response.Wins.Should().BeNull();
        response.TeamId.Should().Be(0);
        response.Updated.Should().BeNull();
        response.Dummy.Should().Be(0);
        response.Year.Should().Be(0);
        response.Round.Should().Be(0);
        response.SourceId.Should().Be(0);
        response.MeanRank.Should().BeNull();
        response.Source.Should().BeNull();
        response.Rank.Should().Be(0);
    }

    [Fact]
    public void LadderResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "team": null,
            "wins": "",
            "updated": null,
            "mean_rank": null,
            "source": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Team.Should().BeNull();
        response.Wins.Should().Be("");
        response.Updated.Should().BeNull();
        response.MeanRank.Should().BeNull();
        response.Source.Should().BeNull();
    }

    [Fact]
    public void LadderResponse_EmptySwarms_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "swarms": []
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Swarms.Should().NotBeNull();
        response.Swarms.Should().BeEmpty();
    }

    [Fact]
    public void LadderResponse_NullSwarms_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "swarms": null
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Swarms.Should().BeNull();
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(18)]
    [InlineData(int.MaxValue)]
    public void LadderResponse_NumericProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "teamid": {{value}},
            "dummy": {{value}},
            "year": {{value}},
            "round": {{value}},
            "sourceid": {{value}},
            "rank": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.TeamId.Should().Be(value);
        response.Dummy.Should().Be(value);
        response.Year.Should().Be(value);
        response.Round.Should().Be(value);
        response.SourceId.Should().Be(value);
        response.Rank.Should().Be(value);
    }

    [Theory]
    [InlineData("Melbourne")]
    [InlineData("Brisbane Lions")]
    [InlineData("")]
    [InlineData("Team with Special Characters !@#")]
    public void LadderResponse_TeamProperty_ShouldHandleVariousValues(string team)
    {
        // Arrange
        var json = $$"""
        {
            "team": "{{team}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Team.Should().Be(team);
    }

    [Theory]
    [InlineData("18")]
    [InlineData("0")]
    [InlineData("22")]
    [InlineData("")]
    public void LadderResponse_WinsProperty_ShouldHandleVariousValues(string wins)
    {
        // Arrange
        var json = $$"""
        {
            "wins": "{{wins}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Wins.Should().Be(wins);
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("10.25")]
    [InlineData("0.0")]
    [InlineData("")]
    public void LadderResponse_MeanRankProperty_ShouldHandleVariousValues(string meanRank)
    {
        // Arrange
        var json = $$"""
        {
            "mean_rank": "{{meanRank}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.MeanRank.Should().Be(meanRank);
    }

    [Theory]
    [InlineData("AFL Official")]
    [InlineData("Expert Analysis")]
    [InlineData("")]
    [InlineData("Source with Special Characters !@#")]
    public void LadderResponse_SourceProperty_ShouldHandleVariousValues(string source)
    {
        // Arrange
        var json = $$"""
        {
            "source": "{{source}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Source.Should().Be(source);
    }

    [Theory]
    [InlineData("2025-06-28T10:30:00Z")]
    [InlineData("2025-12-31T23:59:59Z")]
    [InlineData("")]
    public void LadderResponse_UpdatedProperty_ShouldHandleVariousValues(string updated)
    {
        // Arrange
        var json = $$"""
        {
            "updated": "{{updated}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Updated.Should().Be(updated);
    }

    [Fact]
    public void LadderResponse_SwarmsWithMultipleItems_ShouldBeHandledCorrectly()
    {
        // Arrange
        var json = """
        {
            "swarms": ["tigers", "richmond", "yellow", "black", "mcg"]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Swarms.Should().NotBeNull();
        response.Swarms.Should().HaveCount(5);
        response.Swarms.Should().Equal("tigers", "richmond", "yellow", "black", "mcg");
    }

    [Fact]
    public void LadderResponse_SwarmsWithSingleItem_ShouldBeHandledCorrectly()
    {
        // Arrange
        var json = """
        {
            "swarms": ["demons"]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Swarms.Should().NotBeNull();
        response.Swarms.Should().HaveCount(1);
        response.Swarms.Should().Equal("demons");
    }

    [Theory]
    [InlineData(2020, 1)]
    [InlineData(2025, 23)]
    [InlineData(2030, 15)]
    public void LadderResponse_YearAndRoundCombinations_ShouldBeHandledCorrectly(int year, int round)
    {
        // Arrange
        var json = $$"""
        {
            "year": {{year}},
            "round": {{round}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Year.Should().Be(year);
        response.Round.Should().Be(round);
    }

    [Theory]
    [InlineData(1, 1, 1)] // Team 1, Source 1, Rank 1
    [InlineData(18, 5, 18)] // Team 18, Source 5, Rank 18
    [InlineData(0, 0, 0)] // Edge case with zeros
    public void LadderResponse_IdAndRankCombinations_ShouldBeHandledCorrectly(int teamId, int sourceId, int rank)
    {
        // Arrange
        var json = $$"""
        {
            "teamid": {{teamId}},
            "sourceid": {{sourceId}},
            "rank": {{rank}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<LadderResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.TeamId.Should().Be(teamId);
        response.SourceId.Should().Be(sourceId);
        response.Rank.Should().Be(rank);
    }
}