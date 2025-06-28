using System.Text.Json;
using FluentAssertions;
using mcp_afl_server.Models;

namespace mcp_afl_server.UnitTests.Models;

public class TipsResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void TipsResponse_JsonDeserialization_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var json = """
        {
            "correct": 1,
            "round": 15,
            "year": 2025,
            "updated": "2025-06-28T10:30:00Z",
            "bits": "some_bits_data",
            "err": "",
            "gameid": 12345,
            "source": "AFL Official",
            "tipteamid": 5,
            "ateam": "Carlton Blues",
            "hteam": "Richmond Tigers",
            "hmargin": "12",
            "sourceid": 1,
            "margin": "8",
            "venue": "MCG",
            "hteamid": 1,
            "ateamid": 2,
            "date": "2025-06-28",
            "tip": "Richmond Tigers",
            "hconfidence": "75.5",
            "confidence": "68.2"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Correct.Should().Be(1);
        response.Round.Should().Be(15);
        response.Year.Should().Be(2025);
        response.Updated.Should().Be("2025-06-28T10:30:00Z");
        response.Bits.Should().Be("some_bits_data");
        response.Error.Should().Be("");
        response.GameId.Should().Be(12345);
        response.Source.Should().Be("AFL Official");
        response.TipTeamId.Should().Be(5);
        response.AwayTeam.Should().Be("Carlton Blues");
        response.HomeTeam.Should().Be("Richmond Tigers");
        response.HomeMargin.Should().Be("12");
        response.SourceId.Should().Be(1);
        response.Margin.Should().Be("8");
        response.Venue.Should().Be("MCG");
        response.HomeTeamId.Should().Be(1);
        response.AwayTeamId.Should().Be(2);
        response.Date.Should().Be("2025-06-28");
        response.Tip.Should().Be("Richmond Tigers");
        response.HomeConfidence.Should().Be("75.5");
        response.Confidence.Should().Be("68.2");
    }

    [Fact]
    public void TipsResponse_JsonSerialization_ShouldProduceCorrectJson()
    {
        // Arrange
        var tipsResponse = new TipsResponse
        {
            Correct = 1,
            Round = 15,
            Year = 2025,
            Updated = "2025-06-28T10:30:00Z",
            Bits = "some_bits_data",
            Error = "",
            GameId = 12345,
            Source = "AFL Official",
            TipTeamId = 5,
            AwayTeam = "Carlton Blues",
            HomeTeam = "Richmond Tigers",
            HomeMargin = "12",
            SourceId = 1,
            Margin = "8",
            Venue = "MCG",
            HomeTeamId = 1,
            AwayTeamId = 2,
            Date = "2025-06-28",
            Tip = "Richmond Tigers",
            HomeConfidence = "75.5",
            Confidence = "68.2"
        };

        // Act
        var json = JsonSerializer.Serialize(tipsResponse, _jsonOptions);

        // Assert
        json.Should().Contain("\"correct\":1");
        json.Should().Contain("\"round\":15");
        json.Should().Contain("\"year\":2025");
        json.Should().Contain("\"updated\":\"2025-06-28T10:30:00Z\"");
        json.Should().Contain("\"bits\":\"some_bits_data\"");
        json.Should().Contain("\"err\":\"\"");
        json.Should().Contain("\"gameid\":12345");
        json.Should().Contain("\"source\":\"AFL Official\"");
        json.Should().Contain("\"tipteamid\":5");
        json.Should().Contain("\"ateam\":\"Carlton Blues\"");
        json.Should().Contain("\"hteam\":\"Richmond Tigers\"");
        json.Should().Contain("\"hmargin\":\"12\"");
        json.Should().Contain("\"sourceid\":1");
        json.Should().Contain("\"margin\":\"8\"");
        json.Should().Contain("\"venue\":\"MCG\"");
        json.Should().Contain("\"hteamid\":1");
        json.Should().Contain("\"ateamid\":2");
        json.Should().Contain("\"date\":\"2025-06-28\"");
        json.Should().Contain("\"tip\":\"Richmond Tigers\"");
        json.Should().Contain("\"hconfidence\":\"75.5\"");
        json.Should().Contain("\"confidence\":\"68.2\"");
    }

    [Fact]
    public void TipsResponse_PropertyAssignment_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var response = new TipsResponse
        {
            Correct = 0,
            Round = 12,
            Year = 2025,
            Updated = "2025-07-15T14:20:00Z",
            Bits = "different_bits",
            Error = "No error",
            GameId = 54321,
            Source = "Expert Analysis",
            TipTeamId = 3,
            AwayTeam = "Essendon",
            HomeTeam = "Collingwood",
            HomeMargin = "15",
            SourceId = 2,
            Margin = "10",
            Venue = "Marvel Stadium",
            HomeTeamId = 4,
            AwayTeamId = 6,
            Date = "2025-07-15",
            Tip = "Collingwood",
            HomeConfidence = "82.3",
            Confidence = "79.1"
        };

        // Assert
        response.Correct.Should().Be(0);
        response.Round.Should().Be(12);
        response.Year.Should().Be(2025);
        response.Updated.Should().Be("2025-07-15T14:20:00Z");
        response.Bits.Should().Be("different_bits");
        response.Error.Should().Be("No error");
        response.GameId.Should().Be(54321);
        response.Source.Should().Be("Expert Analysis");
        response.TipTeamId.Should().Be(3);
        response.AwayTeam.Should().Be("Essendon");
        response.HomeTeam.Should().Be("Collingwood");
        response.HomeMargin.Should().Be("15");
        response.SourceId.Should().Be(2);
        response.Margin.Should().Be("10");
        response.Venue.Should().Be("Marvel Stadium");
        response.HomeTeamId.Should().Be(4);
        response.AwayTeamId.Should().Be(6);
        response.Date.Should().Be("2025-07-15");
        response.Tip.Should().Be("Collingwood");
        response.HomeConfidence.Should().Be("82.3");
        response.Confidence.Should().Be("79.1");
    }

    [Fact]
    public void TipsResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new TipsResponse();

        // Assert
        response.Correct.Should().Be(0);
        response.Round.Should().Be(0);
        response.Year.Should().Be(0);
        response.Updated.Should().BeNull();
        response.Bits.Should().BeNull();
        response.Error.Should().BeNull();
        response.GameId.Should().Be(0);
        response.Source.Should().BeNull();
        response.TipTeamId.Should().Be(0);
        response.AwayTeam.Should().BeNull();
        response.HomeTeam.Should().BeNull();
        response.HomeMargin.Should().BeNull();
        response.SourceId.Should().Be(0);
        response.Margin.Should().BeNull();
        response.Venue.Should().BeNull();
        response.HomeTeamId.Should().Be(0);
        response.AwayTeamId.Should().Be(0);
        response.Date.Should().BeNull();
        response.Tip.Should().BeNull();
        response.HomeConfidence.Should().BeNull();
        response.Confidence.Should().BeNull();
    }

    [Fact]
    public void TipsResponse_NullStringProperties_ShouldBeHandled()
    {
        // Arrange
        var json = """
        {
            "updated": null,
            "bits": "",
            "err": null,
            "source": null,
            "ateam": null,
            "hteam": "",
            "hmargin": null,
            "margin": "",
            "venue": null,
            "date": null,
            "tip": "",
            "hconfidence": null,
            "confidence": ""
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Updated.Should().BeNull();
        response.Bits.Should().Be("");
        response.Error.Should().BeNull();
        response.Source.Should().BeNull();
        response.AwayTeam.Should().BeNull();
        response.HomeTeam.Should().Be("");
        response.HomeMargin.Should().BeNull();
        response.Margin.Should().Be("");
        response.Venue.Should().BeNull();
        response.Date.Should().BeNull();
        response.Tip.Should().Be("");
        response.HomeConfidence.Should().BeNull();
        response.Confidence.Should().Be("");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void TipsResponse_NumericProperties_ShouldHandleEdgeCases(int value)
    {
        // Arrange
        var json = $$"""
        {
            "correct": {{value}},
            "round": {{value}},
            "year": {{value}},
            "gameid": {{value}},
            "tipteamid": {{value}},
            "sourceid": {{value}},
            "hteamid": {{value}},
            "ateamid": {{value}}
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Correct.Should().Be(value);
        response.Round.Should().Be(value);
        response.Year.Should().Be(value);
        response.GameId.Should().Be(value);
        response.TipTeamId.Should().Be(value);
        response.SourceId.Should().Be(value);
        response.HomeTeamId.Should().Be(value);
        response.AwayTeamId.Should().Be(value);
    }

    [Theory]
    [InlineData("Richmond Tigers", "Carlton Blues")]
    [InlineData("", "")]
    [InlineData("Team with Special Characters !@#", "Another Team")]
    public void TipsResponse_TeamNames_ShouldHandleVariousValues(string homeTeam, string awayTeam)
    {
        // Arrange
        var json = $$"""
        {
            "hteam": "{{homeTeam}}",
            "ateam": "{{awayTeam}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeTeam.Should().Be(homeTeam);
        response.AwayTeam.Should().Be(awayTeam);
    }

    [Theory]
    [InlineData("12", "8")]
    [InlineData("0", "0")]
    [InlineData("", "")]
    [InlineData("25.5", "-5")]
    public void TipsResponse_MarginProperties_ShouldHandleVariousValues(string homeMargin, string margin)
    {
        // Arrange
        var json = $$"""
        {
            "hmargin": "{{homeMargin}}",
            "margin": "{{margin}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeMargin.Should().Be(homeMargin);
        response.Margin.Should().Be(margin);
    }

    [Theory]
    [InlineData("75.5", "68.2")]
    [InlineData("0.0", "100.0")]
    [InlineData("", "")]
    [InlineData("99.99", "1.01")]
    public void TipsResponse_ConfidenceProperties_ShouldHandleVariousValues(string homeConfidence, string confidence)
    {
        // Arrange
        var json = $$"""
        {
            "hconfidence": "{{homeConfidence}}",
            "confidence": "{{confidence}}"
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<TipsResponse>(json, _jsonOptions);

        // Assert
        response.Should().NotBeNull();
        response!.HomeConfidence.Should().Be(homeConfidence);
        response.Confidence.Should().Be(confidence);
    }
}