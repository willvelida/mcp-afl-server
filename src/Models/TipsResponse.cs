using System.Text.Json.Serialization;

namespace mcp_afl_server.Models
{
    public class TipsResponse
    {
        [JsonPropertyName("correct")]
        public int Correct { get; set; }
        [JsonPropertyName("round")]
        public int Round { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("updated")]
        public string Updated { get; set; }
        [JsonPropertyName("bits")]
        public string Bits { get; set; }
        [JsonPropertyName("err")]
        public string Error { get; set; }
        [JsonPropertyName("gameid")]
        public int GameId { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("tipteamid")]
        public int TipTeamId { get; set; }
        [JsonPropertyName("ateam")]
        public string AwayTeam { get; set; }
        [JsonPropertyName("hteam")]
        public string HomeTeam { get; set; }
        [JsonPropertyName("hmargin")]
        public string HomeMargin { get; set; }
        [JsonPropertyName("sourceid")]
        public int SourceId { get; set; }
        [JsonPropertyName("margin")]
        public string Margin { get; set; }
        [JsonPropertyName("venue")]
        public string Venue { get; set; }
        [JsonPropertyName("hteamid")]
        public int HomeTeamId { get; set; }
        [JsonPropertyName("ateamid")]
        public int AwayTeamId { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("tip")]
        public string Tip { get; set; }
        [JsonPropertyName("hconfidence")]
        public string HomeConfidence { get; set; }
        [JsonPropertyName("confidence")]
        public string Confidence { get; set; }
    }
}
